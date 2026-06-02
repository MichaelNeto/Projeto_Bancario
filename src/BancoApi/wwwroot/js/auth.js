/**
 * RN02 - Gerenciamento de Autenticação e Persistência de JWT
 * Responsável por login, persistência de token e redirecionamento de segurança
 */

const STORAGE_KEY_TOKEN = 'jwt_token';
const STORAGE_KEY_ACCOUNT = 'account_data';
const STORAGE_TYPE = 'localStorage'; // ou 'sessionStorage'

/**
 * Realiza login na API
 * @param {string} documento - CPF ou CNPJ do usuário (sem máscara)
 * @param {string} senha - Senha do usuário
 * @returns {Promise}
 */
async function login(documento, senha) {
    try {
        const response = await apiPost('/auth/login', {
            documento: removeSpecialChars(documento),
            senha: senha
        });
        
        // HTTP 200 - Sucesso no login
        if (response.token) {
            // Persistir token JWT (RN02)
            const accountData = {
                documento: removeSpecialChars(documento),
                token_time: new Date().toISOString()
            };
            saveAuthData(response.token, accountData);
            
            // Carregar dados completos do cliente com o novo token
            try {
                const clienteInfo = await apiGet('/clientes/me', response.token);
                
                // Atualizar dados armazenados com informações completas
                const accountDataCompleto = {
                    documento: removeSpecialChars(documento),
                    nome: clienteInfo.nome || clienteInfo.Nome,
                    numero_conta: clienteInfo.numeroConta || clienteInfo.NumeroConta,
                    numero_agencia: clienteInfo.numeroAgencia || clienteInfo.NumeroAgencia,
                    saldo: clienteInfo.saldo || clienteInfo.Saldo || 0,
                    token_time: new Date().toISOString()
                };
                saveAuthData(response.token, accountDataCompleto);
                console.log('Dados completos do cliente carregados após login:', accountDataCompleto);
            } catch (clientError) {
                console.warn('Erro ao carregar dados completos após login, usando mínimo:', clientError);
                // Continuar mesmo que falhe, os dados serão buscados quando abrir o dashboard
            }
            
            showAlert('Login realizado com sucesso!', 'success');
            
            // Redirecionar para dashboard
            redirect('dashboard.html');
            
            return response;
        }
    } catch (error) {
        console.error('Erro ao fazer login:', error);
        showAlert('Erro ao fazer login: ' + (error.message || 'Tente novamente'), 'error');
        throw error;
    }
}

/**
 * Salva token JWT e dados da conta no storage
 * @param {string} token - Token JWT
 * @param {object} accountData - Dados da conta
 */
function saveAuthData(token, accountData) {
    const storage = STORAGE_TYPE === 'sessionStorage' ? sessionStorage : localStorage;
    
    storage.setItem(STORAGE_KEY_TOKEN, token);
    storage.setItem(STORAGE_KEY_ACCOUNT, JSON.stringify(accountData));
}

/**
 * Obtém o token JWT do storage
 * @returns {string|null}
 */
function getToken() {
    const storage = STORAGE_TYPE === 'sessionStorage' ? sessionStorage : localStorage;
    return storage.getItem(STORAGE_KEY_TOKEN);
}

/**
 * Obtém os dados da conta do storage
 * @returns {object|null}
 */
function getAccountData() {
    const storage = STORAGE_TYPE === 'sessionStorage' ? sessionStorage : localStorage;
    const data = storage.getItem(STORAGE_KEY_ACCOUNT);
    return data ? JSON.parse(data) : null;
}

/**
 * Remove os dados de autenticação
 */
function logout() {
    const storage = STORAGE_TYPE === 'sessionStorage' ? sessionStorage : localStorage;
    
    storage.removeItem(STORAGE_KEY_TOKEN);
    storage.removeItem(STORAGE_KEY_ACCOUNT);
    
    showAlert('Logout realizado com sucesso', 'success');
    redirect('index.html');
}

/**
 * Verifica se o usuário está autenticado
 * @returns {boolean}
 */
function isAuthenticated() {
    const token = getToken();
    return token !== null && token !== '';
}

/**
 * RN02 - Verifica autenticação e redireciona se necessário
 * Chamado nas páginas protegidas (dashboard.html, etc)
 */
function checkAuthentication() {
    if (!isAuthenticated()) {
        // Token JWT não está presente - redirecionar para login (RN02)
        redirect('index.html');
    }
}

/**
 * Inicializa a página verificando autenticação
 * Chamado ao carregar páginas protegidas
 */
function initializeProtectedPage() {
    checkAuthentication();
    
    // Carregar dados da conta
    const accountData = getAccountData();
    if (accountData) {
        displayAccountInfo(accountData);
    }
}

/**
 * Exibe informações da conta na interface
 * @param {object} accountData - Dados da conta
 */
function displayAccountInfo(accountData) {
    const accountNameElement = document.getElementById('account-name');
    const accountNumberElement = document.getElementById('account-number');
    
    if (accountNameElement) {
        accountNameElement.textContent = accountData.nome || 'Usuário';
    }
    
    if (accountNumberElement) {
        accountNumberElement.textContent = 'Conta: ' + (accountData.numero_conta || 'N/A');
    }
}

function validateDocument(documento) {
    // Remove tudo que não for número
    documento = documento.replace(/\D/g, '');

    // CPF = 11 dígitos
    if (documento.length === 11) {
        return { isValid: true, type: 'CPF' };
    }

    // CNPJ = 14 dígitos
    if (documento.length === 14) {
        return { isValid: true, type: 'CNPJ' };
    }

    return { isValid: false };
}

/**
 * Preenche os elementos da página com dados do localStorage
 * Usado como fallback quando a API ainda não retornou dados
 */
function preencherDadosLocalStorage() {
    const accountData = getAccountData();
    if (!accountData) return;
    
    // Preencher nome e número da conta
    if (accountData.nome) {
        const nameElement = document.getElementById('account-name');
        if (nameElement) {
            nameElement.textContent = accountData.nome;
        }
    }
    
    if (accountData.numero_agencia && accountData.numero_conta) {
        const numberElement = document.getElementById('account-number');
        if (numberElement) {
            numberElement.textContent = `Conta: ${accountData.numero_agencia}/${accountData.numero_conta}`;
        }
    }
    
    // Preencher saldo se disponível
    if (accountData.saldo !== undefined && accountData.saldo !== null) {
        const saldoElement = document.getElementById('saldo');
        if (saldoElement) {
            saldoElement.textContent = formatCurrency(accountData.saldo);
            console.log('Saldo preenchido do localStorage:', formatCurrency(accountData.saldo));
        }
    }
}

/**
 * Carrega os dados atualizados do cliente da API
 * Inclui saldo atualizado no banco de dados
 * @returns {Promise}
 */
async function carregarDadosCliente() {

    console.log("🚀 carregarDadosCliente foi chamada");
    try {
        const token = getToken();
        if (!token) {
            console.warn('Token não disponível para carregarDadosCliente');
            return null;
        }

        const clienteInfo = await apiGet('/clientes/me', token);
        console.log('Dados do cliente carregados:', clienteInfo);
        
        // Obter saldo (pode ser saldo ou Saldo dependendo da serialização)
        const saldo = clienteInfo.saldo !== undefined ? clienteInfo.saldo : clienteInfo.Saldo;
        const nome = clienteInfo.nome || clienteInfo.Nome;
        const numeroAgencia = clienteInfo.numeroAgencia || clienteInfo.NumeroAgencia;
        const numeroConta = clienteInfo.numeroConta || clienteInfo.NumeroConta;
        
        // Atualizar saldo na página
        const saldoElement = document.getElementById('saldo');
        if (saldoElement) {
            saldoElement.textContent = formatCurrency(saldo);
            console.log('Saldo atualizado:', formatCurrency(saldo));
        }
        
        // Atualizar informações da conta
        const nameElement = document.getElementById('account-name');
        if (nameElement) {
            nameElement.textContent = nome || 'Usuário';
        }
        
        const numberElement = document.getElementById('account-number');
        if (numberElement) {
            numberElement.textContent = `Conta: ${numeroAgencia}/${numeroConta}`;
        }
        
        // Armazenar dados atualizados
        const accountData = {
            nome: nome,
            numero_conta: numeroConta,
            numero_agencia: numeroAgencia,
            saldo: saldo,
            token_time: new Date().toISOString()
        };
        
        const storage = STORAGE_TYPE === 'sessionStorage' ? sessionStorage : localStorage;
        storage.setItem(STORAGE_KEY_ACCOUNT, JSON.stringify(accountData));
        
        return clienteInfo;
    } catch (error) {
        console.error('Erro ao carregar dados do cliente:', error);
        return null;
    }
}

/**
 * Event listeners padrão
 */
document.addEventListener('DOMContentLoaded', function() {
    // Verificar se há botão de logout
    const logoutBtn = document.getElementById('logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function(e) {
            e.preventDefault();
            logout();
        });
    }
    
    // Verificar se há formulário de login
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const documento = document.getElementById('documento').value;
            const senha = document.getElementById('senha').value;
            
            const docValidation = validateDocument(documento);
            if (!docValidation.isValid) {
                showAlert('CPF ou CNPJ inválido', 'error');
                return;
            }
            
            if (!senha || senha.length < 6) {
                showAlert('Senha deve ter pelo menos 6 caracteres', 'error');
                return;
            }
            
            login(documento, senha);
        });
    }


});
