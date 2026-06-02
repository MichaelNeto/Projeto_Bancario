/**
 * Arquivo utilitário com funções comuns
 */

// Configuração da API
let cachedApiBaseUrl = null;

function isDevelopmentHost() {
    const hostname = window.location.hostname;
    const port = window.location.port;
    const devPorts = ['5500', '5173', '3000', '8080', '8000', '8888'];
    return hostname === 'localhost' || hostname === '127.0.0.1' || devPorts.includes(port);
}

async function detectApiBaseUrl() {
    if (!isDevelopmentHost()) {
        return '/api';
    }

    if (cachedApiBaseUrl) {
        return cachedApiBaseUrl;
    }

    const candidates = ['http://localhost:5052/api', 'http://localhost:5000/api'];

    for (const candidate of candidates) {
        try {
            const response = await fetch(candidate, { method: 'GET', mode: 'cors', cache: 'no-store' });
            if (response) {
                cachedApiBaseUrl = candidate;
                return candidate;
            }
        } catch (error) {
            // host não acessível; testa próximo
        }
    }

    // fallback seguro para localhost:5052
    cachedApiBaseUrl = 'http://localhost:5052/api';
    return cachedApiBaseUrl;
}

/**
 * Realiza uma requisição GET
 * @param {string} endpoint - Endpoint da API
 * @param {string} token - Token JWT (opcional)
 * @returns {Promise}
 */
async function apiGet(endpoint, token = null) {
    const apiBaseUrl = await detectApiBaseUrl();
    const headers = {
        'Content-Type': 'application/json'
    };
    
    if (token) {
        headers['Authorization'] = 'Bearer ' + token;
    }
    
    const url = apiBaseUrl + endpoint;
    console.log('🔍 GET Request:', { url, headers });

    const response = await fetch(url, {
        method: 'GET',
        headers: headers
    });
    
    console.log(`📨 GET Response Status: ${response.status} ${response.statusText}`);
    const data = await handleApiResponse(response);
    console.log(`📦 GET Response Data:`, data);
    return data;
}

/**
 * Realiza uma requisição POST
 * @param {string} endpoint - Endpoint da API
 * @param {object} data - Dados a enviar
 * @param {string} token - Token JWT (opcional)
 * @returns {Promise}
 */
async function apiPost(endpoint, data, token = null) {
    const headers = {
        'Content-Type': 'application/json'
    };
    
    if (token) {
        headers['Authorization'] = 'Bearer ' + token;
    }
    
    const apiBaseUrl = await detectApiBaseUrl();
    const url = apiBaseUrl + endpoint;
    console.log('🔍 POST Request:', { url, data, headers });
    
    const response = await fetch(url, {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(data)
    });
    
    console.log('📬 Response status:', response.status, response.statusText);
    return handleApiResponse(response);
}

/**
 * Trata a resposta da API
 * @param {Response} response - Resposta HTTP
 * @returns {Promise}
 */
async function handleApiResponse(response) {
    const contentType = response.headers.get('Content-Type') || '';
    console.log(`🔄 Content-Type: ${contentType}`);
    
    let data = null;
    let text = null;

    if (contentType.includes('application/json')) {
        try {
            data = await response.json();
            console.log(`✅ JSON parseado com sucesso`);
        } catch (error) {
            console.error(`❌ Erro ao fazer parse de JSON:`, error);
            text = await response.text();
        }
    } else {
        text = await response.text();
        if (text) {
            try {
                data = JSON.parse(text);
                console.log(`✅ Text convertido para JSON com sucesso`);
            } catch (error) {
                console.log(`⚠️ Text não é JSON válido`);
                data = null;
            }
        }
    }

    if (!response.ok) {
        const errorMessage = data?.mensagem || data?.message || text || response.statusText || 'Erro na requisição';
        console.error(`🚫 Erro HTTP ${response.status}:`, errorMessage);
        throw {
            status: response.status,
            message: errorMessage,
            data: data ?? text
        };
    }

    return data;
}

/**
 * Formata valor monetário em Real
 * @param {number} value - Valor a formatar
 * @returns {string}
 */
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

/**
 * Formata data para formato brasileiro
 * @param {string|Date} date - Data a formatar
 * @returns {string}
 */
function formatDate(date) {
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    });
}

/**
 * Remove caracteres especiais de uma string
 * @param {string} str - String a limpar
 * @returns {string}
 */
function removeSpecialChars(str) {
    return str.replace(/[^a-zA-Z0-9]/g, '');
}

/**
 * Valida email
 * @param {string} email - Email a validar
 * @returns {boolean}
 */
function isValidEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

/**
 * Redireciona para uma página
 * @param {string} page - Nome da página (ex: 'index.html')
 */
function redirect(page) {
    window.location.href = '/' + page;
}

/**
 * Exibe mensagem de alerta
 * @param {string} message - Mensagem a exibir
 * @param {string} type - Tipo de alerta ('success', 'error', 'warning', 'info')
 */
function showAlert(message, type = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-' + type;
    alertDiv.textContent = message;
    
    const container = document.querySelector('body');
    container.insertBefore(alertDiv, container.firstChild);
    
    setTimeout(() => {
        alertDiv.remove();
    }, 5000);
}

function isValidCPF(cpf) {
    const cleaned = removeSpecialChars(cpf);
    if (cleaned.length !== 11 || !/^\d+$/.test(cleaned)) return false;
    if (/^(\d)\1{10}$/.test(cleaned)) return false;
    return true;
}

function isValidCNPJ(cnpj) {
    const cleaned = removeSpecialChars(cnpj);
    if (cleaned.length !== 14) return false;
    if (!/^\d+$/.test(cleaned.slice(12, 14))) return false;
    return true;
}

function validateDocument(documento) {
    const cleaned = removeSpecialChars(documento);
    if (cleaned.length === 11) return { isValid: isValidCPF(cleaned), type: 'CPF' };
    if (cleaned.length === 14) return { isValid: isValidCNPJ(cleaned), type: 'CNPJ' };
    return { isValid: false, type: 'INVALID' };
}
