# ⛏️ Mining Boost

> Um mod para **WorldBox** que multiplica a quantidade de recursos obtidos durante a mineração e adiciona um sistema opcional de eventos caóticos que podem alterar completamente o rumo de um mundo.

O **Mining Boost** aumenta a quantidade de recursos coletados sempre que uma unidade extrai materiais de construções.

Por padrão, cada recurso minerado é multiplicado em **100×** antes de ser adicionado ao inventário do trabalhador. Além disso, o mod inclui um sistema opcional chamado **Minas Instáveis**, que pode espalhar loucura, peste ou corrupção pelo mundo, criando eventos imprevisíveis durante a jogatina.

---

# ✨ Funcionalidades

* 🚀 Multiplicador de recursos totalmente configurável (1× até 10000×)
* ⚙️ Configuração através do BepInEx (sem necessidade de recompilar)
* ☣️ Sistema opcional de Minas Instáveis
* 🦠 Eventos aleatórios de loucura, peste e corrupção
* 🧩 Implementado utilizando Harmony
* 💾 Leve e com baixo impacto no desempenho
* 🎮 Compatível com a versão para PC do WorldBox

---

# 📦 Requisitos

Antes de instalar o mod, certifique-se de possuir:

* WorldBox para PC;
* BepInEx instalado;
* .NET SDK com suporte ao **.NET Framework 4.7.2**;
* Linux (ou ajuste os caminhos de referência em `MiningBoost.csproj` para sua instalação).

---

# 🚀 Instalação

## 1. Compile o projeto

```bash
dotnet build -c Release
```

---

## 2. Instale o mod

Copie o arquivo

```text
bin/Release/net472/MiningBoost.dll
```

para

```text
WorldBox/BepInEx/plugins/
```

---

## 3. Execute o jogo

Se tudo estiver correto, o log do BepInEx exibirá uma mensagem semelhante a:

```text
Mining Boost loaded with multiplier 100x and mining chaos at 5%.
```

---

# ⚙️ Configuração

Após a primeira execução, o BepInEx criará automaticamente o arquivo:

```text
BepInEx/config/castro_war.miningboost.cfg
```

O multiplicador pode ser alterado livremente, sem necessidade de recompilar o projeto.

```ini
[General]
Multiplier = 100
```

### Valores disponíveis

| Valor | Resultado                      |
| ----- | ------------------------------ |
| 1     | Comportamento original do jogo |
| 10    | Recursos multiplicados por 10  |
| 100   | Valor padrão                   |
| 1000  | Produção extremamente elevada  |
| 10000 | Valor máximo suportado         |

O intervalo permitido vai de **1** até **10000**.

---

# ☣️ Minas Instáveis

Além do multiplicador de recursos, o mod pode adicionar eventos aleatórios durante a mineração.

Cada extração possui uma chance configurável de corromper o trabalhador com um dos seguintes traços:

| Evento                 | Descrição                                           |
| ---------------------- | --------------------------------------------------- |
| 🧠 Loucura (`madness`) | Pode separar o personagem e criar um reino insano.  |
| ☣️ Peste (`plague`)    | Inicia ou amplia surtos de peste pelo mundo.        |
| 😈 Corrupção (`evil`)  | Torna a unidade mais agressiva e menos diplomática. |

A configuração é feita no mesmo arquivo:

```ini
[Mining Chaos]

Enabled = true

ChancePercent = 5
```

### Configurações

| Opção         | Descrição                                                          |
| ------------- | ------------------------------------------------------------------ |
| Enabled       | Ativa ou desativa o sistema de Minas Instáveis                     |
| ChancePercent | Probabilidade (0–100%) de um evento ocorrer durante cada mineração |

Caso deseje utilizar apenas o multiplicador de recursos, basta configurar:

```ini
Enabled = false
```

ou

```ini
ChancePercent = 0
```

---

# 🏗️ Desenvolvimento

O projeto utiliza a propriedade MSBuild:

```text
WorldBoxPath
```

para localizar automaticamente a instalação do jogo.

Caso sua instalação esteja em outro diretório, compile utilizando:

```bash
dotnet build -c Release -p:WorldBoxPath=/caminho/para/worldbox
```

---

# 🔧 Como o mod funciona

O **Mining Boost** utiliza a biblioteca **Harmony** para modificar o comportamento do jogo através de um **Transpiler**.

Ao invés de substituir completamente o sistema de mineração do WorldBox, o mod intercepta a chamada para:

```text
Actor.addToInventory(...)
```

e a redireciona para uma implementação própria.

Esse método:

* multiplica automaticamente a quantidade de recursos;
* limita os valores ao intervalo permitido pelo `Int32`, evitando estouros numéricos;
* preserva os rótulos e o fluxo original do código IL;
* executa o sistema de Minas Instáveis (quando habilitado);
* adiciona os recursos ao inventário normalmente.

Essa abordagem reduz conflitos com outras partes do jogo e mantém a lógica original praticamente intacta.

---

# 📈 Desempenho

O mod realiza apenas algumas operações simples durante cada mineração:

* multiplicação de inteiros;
* uma verificação aleatória (caso Minas Instáveis estejam ativadas);
* aplicação opcional de um traço ao trabalhador.

Na prática, o impacto no desempenho é praticamente imperceptível.

---

# ⚠️ Compatibilidade

Este é um **mod não oficial** para o WorldBox.

Atualizações futuras do:

* WorldBox;
* BepInEx;
* Harmony;

podem alterar métodos internos do jogo e exigir adaptações para manter a compatibilidade.

---

# 🤝 Contribuição

Sugestões, correções e melhorias são sempre bem-vindas.

Caso encontre algum problema ou tenha ideias para novas funcionalidades, fique à vontade para abrir uma **Issue** ou enviar um **Pull Request**.

---

# 📄 Licença

Este projeto é distribuído como um mod de código aberto para WorldBox.

Sinta-se à vontade para estudar, modificar e contribuir com o projeto, respeitando os termos da licença adotada pelo repositório.
