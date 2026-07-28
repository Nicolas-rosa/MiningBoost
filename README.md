# Mining Boost

Mod para **WorldBox** que aumenta a quantidade de recursos obtidos quando uma unidade extrai recursos de construções.

Atualmente, o multiplicador padrão é **100x**: cada unidade de recurso adicionada ao inventário durante a mineração é multiplicada por 100.

## Requisitos

- WorldBox para PC com o **BepInEx** instalado;
- .NET SDK com suporte ao alvo **.NET Framework 4.7.2**;
- Linux (ou ajuste os caminhos de referência no arquivo `MiningBoost.csproj` para a instalação do seu jogo).

## Instalação

1. Compile o projeto:

   ```bash
   dotnet build -c Release
   ```

2. Copie o arquivo gerado `bin/Release/net472/MiningBoost.dll` para a pasta `BepInEx/plugins` da instalação do WorldBox.

3. Inicie o jogo. A mensagem `MiningBoost iniciado!` deve aparecer no log do BepInEx.

## Configuração

Altere o valor de `Multiplier` em `Plugin.cs` para definir o aumento desejado:

```csharp
public static int Multiplier = 100;
```

Por exemplo, use `10` para obter dez vezes mais recursos. Após alterar o valor, compile novamente e substitua a DLL na pasta de plugins.

## Desenvolvimento

As referências do BepInEx, Harmony e das DLLs do jogo estão configuradas com caminhos locais em `MiningBoost.csproj`. Caso o WorldBox esteja instalado em outro local, atualize os elementos `HintPath` antes de compilar.

O mod usa Harmony para aplicar um *transpiler* ao método `ai.behaviours.BehExtractResourcesFromBuilding.execute`, multiplicando a quantidade enviada para `Actor.addToInventory`.

## Aviso

Este é um mod não oficial e pode precisar de ajustes após atualizações do WorldBox ou do BepInEx.
