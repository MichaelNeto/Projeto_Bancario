# 🏦 Banco - Front-end JavaScript

## 📋 Resumo da Implementação

Implementação completa da interface do usuário e comportamento do front-end para aplicação bancária em JavaScript vanilla, atendendo a todas as Regras de Negócio (RN) e Critérios de Aceitação (CA) especificados.

## 📁 Estrutura de Arquivos

```
wwwroot/
├── index.html              # Página de Login
├── cadastro.html           # Página de Cadastro
├── dashboard.html          # Dashboard com Extrato e PIX
├── teste.html              # Guia de Testes
├── css/
│   └── styles.css          # Estilos Responsivos
└── js/
    ├── utils.js            # Funções Utilitárias
    ├── auth.js             # Autenticação (RN02)
    ├── cadastro.js         # Máscara de Documento (RN01)
    └── pix.js              # PIX e Extrato (RN03, RN04)
```

## 🎯 Regras de Negócio Implementadas

### RN01 - Máscara do Novo CNPJ Alfanumérico
- ✅ Identifica automaticamente CPF (11 caracteres) ou CNPJ (14 caracteres)
- ✅ Máscara CPF: `XXX.XXX.XXX-XX`
- ✅ Máscara CNPJ: `XX.XXX.XXX/XXXX-XX` com suporte a letras nas primeiras 12 posições
- ✅ Bloqueia caracteres especiais durante digitação
- **Arquivo:** `cadastro.js`

### RN02 - Persistência do Estado de Login
- ✅ Salva Token JWT em `localStorage` ou `sessionStorage`
- ✅ Salva dados da conta no navegador
- ✅ Redireciona automaticamente para login se token ausente
- ✅ Verifica autenticação ao carregar páginas protegidas
- **Arquivo:** `auth.js`

### RN03 - Envio Autorizado de Requisições
- ✅ Inclui Token JWT em todas as requisições protegidas
- ✅ Header: `Authorization: Bearer {token}`
- ✅ Endpoints: Saldo, Extrato, Enviar PIX
- **Arquivo:** `pix.js`

### RN04 - Renderização Dinâmica do Extrato
- ✅ Processa array de transações do C#
- ✅ Aplica classes CSS dinâmicas:
  - `.ganho` - PIX Recebido (verde com +)
  - `.gasto` - PIX Enviado (vermelho com -)
  - `.estornado` - Status FALHOU (cinza com risco)
- ✅ Exibe motivo_falha para transações falhadas
- **Arquivo:** `pix.js` + `styles.css`

## ✅ Critérios de Aceitação Implementados

### CA01 - Bloqueio de Símbolos no Documento
- ✅ Intercepta caracteres especiais (., -, /)
- ✅ Impede inserção e exibe alerta
- ✅ Máscara aplicada automaticamente

### CA02 - Redirecionamento de Segurança
- ✅ Verifica presença do Token JWT
- ✅ Redireciona para index.html se ausente
- ✅ Previne acesso a URLs protegidas

### CA03 - Destino Visual do Extrato
- ✅ PIX recebido (R$ 350,00) exibido como "+ R$ 350,00" em verde
- ✅ PIX enviado exibido como "- R$ XXX,XX" em vermelho
- ✅ Transações falhadas exibidas com risco e status cinza

## 🚀 Como Usar

### 1. **Login**
```
URL: http://localhost:5000/
- Digitar CPF ou CNPJ
- Digitar Senha
- Clicar em "Entrar"
```

### 2. **Cadastro**
```
URL: http://localhost:5000/cadastro.html
- Preencher Nome, Email, Documento (com máscara automática)
- Preencher Telefone, Endereço, Data de Nascimento
- Digitar Senha (mín. 6 caracteres)
- Confirmar Senha
- Clicar em "Cadastrar"
```

### 3. **Dashboard**
```
URL: http://localhost:5000/dashboard.html
- Visualizar Saldo Disponível
- Enviar PIX (com validação de chave)
- Consultar Extrato com cores dinâmicas
- Logout para sair
```

## 🔒 Segurança

- **Armazenamento de Token:** `localStorage` com opção para `sessionStorage`
- **Validação:** Todos os campos validados antes de envio
- **CORS:** Configurado em Program.cs para aceitar requisições
- **Autenticação:** Verificação obrigatória em páginas protegidas
- **Logout:** Limpa todos os dados de sessão

## 📱 Responsividade

- ✅ Desktop (1200px+)
- ✅ Tablet (768px - 1199px)
- ✅ Mobile (< 768px)
- ✅ Flexbox e CSS Grid
- ✅ Media Queries para adaptação

## 🧪 Testes

Acesse a página de testes para validar cada RN e CA:
```
URL: http://localhost:5000/teste.html
```

Contém guia completo com passos de teste e resultados esperados.

## 🛠 Desenvolvimento

### Dependências
- Nenhuma dependência externa - JavaScript Vanilla
- Compatibilidade: Todos os navegadores modernos (ES6+)

### Scripts Carregados
```html
<script src="/js/utils.js"></script>      <!-- Funções comuns -->
<script src="/js/auth.js"></script>       <!-- Autenticação -->
<script src="/js/cadastro.js"></script>   <!-- Máscaras -->
<script src="/js/pix.js"></script>        <!-- PIX e Extrato -->
```

### Variáveis de Configuração

Em `utils.js`:
```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

Em `auth.js`:
```javascript
const STORAGE_TYPE = 'localStorage'; // ou 'sessionStorage'
```

## 📊 Estrutura de Dados

### LocalStorage
```javascript
{
    jwt_token: "eyJ...",
    account_data: {
        documento: "12345678901",
        token_time: "2025-05-19T10:30:00.000Z"
    }
}
```

### Response Login (C#)
```json
{
    "token": "eyJ..."
}
```

### Response Extrato (C#)
```json
{
    "transacoes": [
        {
            "tipo": "PIX_RECEBIDO",
            "status": "CONCLUIDO",
            "valor": 350.00,
            "data": "2025-05-19T10:30:00",
            "descricao": "Pagamento"
        }
    ]
}
```

## 🐛 Troubleshooting

### Token não persiste
- Verificar localStorage em DevTools (F12 → Application → Storage)
- Verificar STORAGE_TYPE em auth.js

### Requisições retornam 401
- Verificar se token está no header Authorization
- Verificar se token está expirado
- Verificar configuração de CORS no Program.cs

### Máscara não funciona
- Verificar if validateDocument() está importado
- Verificar eventos de input no formulário

### Páginas protegidas redirecionam
- Verificar se Token JWT existe no storage
- Verificar checkAuthentication() na página protegida

## 📝 Notas

- Todos os scripts usam `async/await` para operações assíncronas
- Tratamento de erros com try/catch e alertas ao usuário
- Validação de dados tanto no front como no back
- Suporte a múltiplas formas de PIX (Email, CPF, CNPJ, Telefone)
- Atualização automática de saldo e extrato a cada 30 segundos

## 📄 Licença

Projeto desenvolvido para sistema bancário - Banco.
