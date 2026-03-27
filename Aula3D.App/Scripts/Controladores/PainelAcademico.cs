using Godot;

/// <summary>
/// Painel de controle da interface gráfica.
/// Exibe o botão de carregamento, toggle de câmera e status computacional.
/// </summary>
public partial class PainelAcademico : CanvasLayer
{
	private Button _btnLoadLocal;
	
	private Label[] _huLabels = new Label[7];
	private VBoxContainer _huContainer;
	private Button _btnSaveAberta;
	private Button _btnSaveFechada;
	private Label _lblStatusGesto;
	private Button _btnToggleCamera;

	[Signal] public delegate void OnLoadLocalRequestedEventHandler();
	[Signal] public delegate void OnSaveAbertaRequestedEventHandler();
	[Signal] public delegate void OnSaveFechadaRequestedEventHandler();

	public override void _Ready()
	{
		_btnLoadLocal = GetNodeOrNull<Button>("Panel/BtnLoadLocal");

		if (_btnLoadLocal != null) 
		{
			_btnLoadLocal.Pressed += OnBtnLoadLocalPressed;
		}

		var vbox = GetNodeOrNull<VBoxContainer>("Panel/VBoxContainer");
		if (vbox != null)
		{
			// Remove the old sliders that took too much screen space
			foreach (Node child in vbox.GetChildren())
			{
				child.QueueFree();
			}

			vbox.AddChild(new HSeparator());
			_lblStatusGesto = new Label { 
				Text = "Gesto Atual: AGUARDANDO", 
				HorizontalAlignment = HorizontalAlignment.Center,
				SelfModulate = new Color(1, 1, 0)
			};
			vbox.AddChild(_lblStatusGesto);
			vbox.AddChild(new HSeparator());

			var titleHu = new Label { Text = "Momentos de Hu", HorizontalAlignment = HorizontalAlignment.Center };
			vbox.AddChild(titleHu);
			
			_huContainer = new VBoxContainer();
			vbox.AddChild(_huContainer);
			for (int i = 0; i < 7; i++)
			{
				_huLabels[i] = new Label { Text = $"Hu[{i}]: 0.0" };
				_huContainer.AddChild(_huLabels[i]);
			}
			
			var hboxBtns = new HBoxContainer();
			vbox.AddChild(hboxBtns);
			
			_btnSaveAberta = new Button { Text = "Salvar: ABERTA", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
			_btnSaveAberta.Pressed += OnSaveAbertaPressed;
			hboxBtns.AddChild(_btnSaveAberta);
			
			_btnSaveFechada = new Button { Text = "Salvar: FECHADA", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
			_btnSaveFechada.Pressed += OnSaveFechadaPressed;
			hboxBtns.AddChild(_btnSaveFechada);
			
			vbox.AddChild(new HSeparator());

			_btnToggleCamera = new Button { Text = "Ocultar/Mostrar Câmera" };
			_btnToggleCamera.Pressed += OnBtnToggleCameraPressed;
			vbox.AddChild(_btnToggleCamera);

			if (_btnLoadLocal != null)
			{
				_btnLoadLocal.GetParent()?.RemoveChild(_btnLoadLocal);
				vbox.AddChild(new HSeparator());
				vbox.AddChild(_btnLoadLocal);
			}

			vbox.AddChild(new HSeparator());
			var lblDicas = new Label { 
				Text = "Atalhos\n[1-5] Visões OpenCV • [A/F] Salvar Gesto", 
				HorizontalAlignment = HorizontalAlignment.Center,
				Modulate = new Color(0.6f, 0.6f, 0.6f)
			};
			lblDicas.AddThemeFontSizeOverride("font_size", 12);
			vbox.AddChild(lblDicas);
		}

		DisableAllClipping();
	}

	private void OnBtnLoadLocalPressed()
	{
		EmitSignal(SignalName.OnLoadLocalRequested);
	}

	private void OnSaveAbertaPressed()
	{
		EmitSignal(SignalName.OnSaveAbertaRequested);
	}

	private void OnSaveFechadaPressed()
	{
		EmitSignal(SignalName.OnSaveFechadaRequested);
	}

	private void OnBtnToggleCameraPressed()
	{
		var cameraPreview = GetNodeOrNull<TextureRect>("CameraPreview");
		if (cameraPreview != null)
		{
			cameraPreview.Visible = !cameraPreview.Visible;
		}
	}

	public void AtualizarStatusGesto(bool maoAberta)
	{
		if (_lblStatusGesto != null)
		{
			_lblStatusGesto.Text = maoAberta ? "Gesto Atual: ABERTA" : "Gesto Atual: FECHADA";
			_lblStatusGesto.SelfModulate = maoAberta ? new Color(0, 1, 0) : new Color(1, 0.2f, 0.2f);
		}
	}

	public void AtualizarHu(double[] hu)
	{
		if (hu == null) return;
		for (int i = 0; i < 7; i++)
		{
			if (_huLabels[i] != null)
				_huLabels[i].Text = $"Hu[{i}]: {hu[i]:F4}";
		}
	}

	private static void DisableAllClipping()
	{
		RenderingServer.GlobalShaderParameterSet("plane_distance_x", 10000.0f);
		RenderingServer.GlobalShaderParameterSet("plane_distance_y", 10000.0f);
		RenderingServer.GlobalShaderParameterSet("plane_distance_z", 10000.0f);
	}
}
