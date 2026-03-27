# Aula 3D - Visualizador e Interação de Objetos 3D com Visão Computacional

Este projeto é um visualizador de objetos 3D desenvolvido na Godot Engine utilizando C#. Ele permite carregar modelos 3D em tempo de execução e manipulá-los (escala, translação, rotação) quase que inteiramente através de **Visão Computacional (Rastreamento de Mãos)**, oferecendo uma interface acadêmica focada na análise em tempo real dos Momentos de Hu.

## Arquitetura Geral do Projeto

A solução é dividida em quatro frentes principais:
- **Aula3D.App**: O projeto principal Godot que gerencia o visualizador 3D, a UI minimalista focada na webcam e a aplicação das transformações baseadas nos inputs visuais.
- **Aula3D.VisionCore**: A biblioteca central em .NET responsável pelo pipeline de Visão Computacional (processamento de imagem, extração de contornos e verificação via Momentos de Hu ou Defeitos de Convexidade).
- **Aula3D.VisionConsole**: Uma aplicação de console isolada para debugar o pipeline OpenCV.
- **Aula3D.Tests**: Suíte de testes unitários (xUnit) para validação matemática dos algoritmos do classificador sem requerer o motor gráfico.

*Consulte o arquivo [Instructions.md](Instructions.md) para ler documentações detalhadas sobre o funcionamento da arquitetura e PDI.*

## Funcionalidades
- **Carregamento Dinâmico de Modelos:** Suporte em runtime para arquivos `.gltf` e `.glb` utilizando o diálogo nativo do SO.
- **Manipulação por Visão:** Rotacione, posicione e escale modelos utilizando os gestos da sua mão capturados na webcam.
- **Matemática Avançada de Rotação:** Utiliza Quaternions para rotações livres contínuas, imunes à trava de eixos (Gimbal Lock).
- **Detecção Avançada (Machine Learning Leve):** Diferencie os gestos salvando a própria assinatura de sua mão utilizando Distância Euclidiana contra capturas persistidas em um arquivo `gestures.json`.
- **Integração Desacoplada (Facade):** O processamento de visão roda numa thread .NET separada provendo estados diretos para o Godot Physics Loop.

## Pré-requisitos
Para compilar e rodar este projeto, você precisará de:
1. **Godot Engine:** Versão da Godot 4.x **com suporte ao .NET (Mono C#)**.
2. **.NET SDK:** Versão 10.0 (ou compatível) instalada.
3. **Webcam**: Obrigatório para interagir com os modelos.

## Como Fazer Build e Rodar

### Rodando o Visualizador Godot (Aula3D.App)
```bash
cd Aula3D.App
dotnet build
godot-mono --path .
```

### Rodando a Ferramenta de CLI de Visão (Aula3D.VisionConsole)
```bash
cd Aula3D.VisionConsole
dotnet run
```

### Testando Algoritmos Euclidiano/Gestos (Aula3D.Tests)
```bash
cd Aula3D.Tests
dotnet test
```

## Estrutura de Pastas
```text
/Aula3D
├── Aula3D.App/                  # Interface mínima Godot (Modelos 3D e Input)
├── Aula3D.VisionCore/           # Core OpenCV Distância Euclidiana e Momentos Hu
├── Aula3D.VisionConsole/        # App CLI para diagnostico
└── Aula3D.Tests/                # Testes de regressão (xUnit)
```
