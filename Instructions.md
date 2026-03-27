# Instruções Globais e Arquitetura Detalhada - Aula3D

Este documento detalha o que compõe o projeto **Aula3D**, explicando a lógica, a arquitetura e as decisões matemáticas de cada módulo. A intenção é documentar a fundo as implementações do rastreamento de mão, a integração com OpenCV, e a manipulação 3D (Quaternions).

## 1. Visão Geral da Arquitetura

O projeto adota uma arquitetura fracamente acoplada ("Decoupled Architecture") entre os sistemas de visão computacional e o motor 3D. 

- O **Motor 3D (Godot, via `Aula3D.App`)** atua unicamente como consumidor de um estado. Ele não processa imagens da câmera, apenas recebe *posições (X, Y, Z)* e um estado booleano (*Aberta/Fechada*).
- O **Módulo de Visão (`Aula3D.VisionCore`)** processa as imagens em tempo real em uma Background Thread, isolando a performance computacional intensiva do motor principal Godot.
- O **Módulo de Testes (`Aula3D.Tests`)** aplica validações matemáticas via xUnit para certificar precisão da detecção independente do motor gráfico.

## 2. Aula3D.App (O Motor Godot)

### a. `Objeto3D.cs`
A classe crucial em Godot para a lógica de manipulação. Conecta-se através da injeção do provider (`IGestureProvider`) no `_Ready()`.

A cada update (no `_Process(double delta)`):
1. **Mão Aberta (Rotação Livre com Quaternions)**: A movimentação `(Delta X/Y)` transposta em eixos espaciais causaria **Gimbal Lock** usando Euler tradicional. Portanto, matrizes normais `Vector3.Up` e `Vector3.Right` são convertidas em **Quaternions**, garantindo giro geométrico natural e esférico da malha no espaço 3D.
2. **Mão Fechada (Translação/Escala)**: Aplica-se empuxo em `Transform.Origin` puxando as coordenadas da mão interpretadas pela máscara delimitadora de espaço.

### b. `GameManager.cs` e `PainelAcademico.cs` (Interface Baseada em CV)
Antigos "sliders" de translação pura foram descontinuados em prol de focar 100% no motor de visão como controle. O painel hoje expõe estritamente os resultados de pipeline, os vetores dos Momentos de Hu atuais, troca atalhos multiplexados usando arrays de frame e conta com botões interativos para Salvar a Assinatura capturada num JSON local. O FileDialog do OS é acionado via script C# pelo GameManager para carregar glTF dinâmicos na árvore sobre o motor.

## 3. Aula3D.VisionCore (Processamento Digital de Imagem)

### a. Pipeline de Processamento (`FiltroEspacial.cs`)
1. **Filtro Bilateral**: Destrói poros e ruídos microtexturais da pele, mas protege contornos físicos duros de se degradarem.
2. **Equalização de Histograma (CLAHE)**: Através das tonalidades de Saturação, balanceia a iluminação lateral que incide na mão da webcam via espaço de cor HSV.
3. **Máscara & Morfologia (`inRange`)**: Binariza o processamento e fecha frestas falsas na renderização (Dilação).
4. **Filtro Frequencial (Opcional - FFT 2D)**: Tenta resgatar dados limitados do espetro complexo em sensores muito baratos/granulativos.

### b. `ExtratorHu.cs` e Classificação por Distância Euclidiana (Prioridade)
A versão final se apoia fortemente na invariância geométrica garantida pelos **7 Momentos Invariantes de Hu**, que medem o espaço topológico de modo à ser agnóstico à distância da mão para lente. 
O app salva sua base lógica no `gestures.json`. Durante a execução, ele processa a matriz de Hu em tempo real, compara a **Distância Euclidiana** via thresholds limitados pré-calculados contra a rede armazenada, obtendo se a assinatura bate com dados de mão ABERTA ou FECHADA matematicamente.

### c. `ClassificadorDeGestos.cs` (Fallback)
Usando a clássica topologia iterável sem preenchimento computacional, caso o limitador JSON não ache equivalentes rígidas Euclidiana em Hu, o classificador recua para validar as depressões (`Defeitos de Convexidade`). Ele envolve a malha com Convex Hull, triangula do "fundo web" aos dedões, e aplica rigorosamente a *Lei dos Cossenos*, medindo com aproximação se ângulos muito vivos referenciam dedos esticados (dedos contam acima do threshold).
