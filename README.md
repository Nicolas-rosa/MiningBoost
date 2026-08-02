# Mining Boost

Mod para **WorldBox** que aumenta a quantidade de recursos obtidos quando uma unidade extrai recursos de construções.

O multiplicador padrão é **100x**: cada unidade de recurso extraída de uma construção é multiplicada por 100 antes de entrar no inventário da unidade. Minas também podem causar eventos de instabilidade que mudam o rumo de reinos inteiros.

## Requisitos

- WorldBox para PC com o **BepInEx** instalado;
- .NET SDK com suporte ao alvo **.NET Framework 4.7.2**;
- Linux (ou ajuste os caminhos de referência no arquivo `MiningBoost.csproj` para a instalação do seu jogo).

## Instalação

1. Compile o projeto:

   ```bash
   dotnet build -c Release
   ```

2. Copie somente o arquivo gerado `bin/Release/net472/MiningBoost.dll` para a pasta `BepInEx/plugins` da instalação do WorldBox.

3. Inicie o jogo. A mensagem `Mining Boost loaded with multiplier 100x and mining chaos at 5%.` deve aparecer no log do BepInEx.

## Configuração

Após a primeira inicialização, o BepInEx cria o arquivo `BepInEx/config/castro_war.miningboost.cfg`. Altere o valor abaixo e reinicie o jogo:

```ini
[General]
Multiplier = 100
```

Use valores de `1` a `10000`; por exemplo, `10` fornece dez vezes mais recursos. Não é necessário recompilar o mod para alterar a configuração.

### Minas Instáveis

Por padrão, cada extração tem **5%** de chance de corromper o trabalhador com um traço aleatório:

- `madness`: pode separar o ator em um reino insano;
- `plague`: inicia ou amplia surtos de peste;
- `evil`: torna o ator mais agressivo e menos diplomático.

Controle esse sistema no mesmo arquivo de configuração:

```ini
[Mining Chaos]
Enabled = true
ChancePercent = 5
```

`ChancePercent` aceita valores de `0` a `100`. Defina `Enabled = false` ou a chance como `0` para usar somente o multiplicador de recursos.

## Desenvolvimento

As referências do BepInEx, Harmony e das DLLs do jogo usam a propriedade `WorldBoxPath`. Para compilar com uma instalação em outro local, informe-a na linha de comando:

```bash
dotnet build -c Release -p:WorldBoxPath=/caminho/para/worldbox
```

O mod usa Harmony para aplicar um *transpiler* ao método `ai.behaviours.BehExtractResourcesFromBuilding.execute`. Ele substitui a chamada a `Actor.addToInventory` por um método do mod com a mesma assinatura, preservando os rótulos de IL. Esse método multiplica a quantidade, adiciona o recurso e então avalia a instabilidade. O resultado é limitado ao intervalo de `Int32` para evitar estouro numérico.

## Aviso

Este é um mod não oficial e pode precisar de ajustes após atualizações do WorldBox ou do BepInEx.
