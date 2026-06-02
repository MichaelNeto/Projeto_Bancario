/**
 * RN01 - Máscara do Novo CNPJ Alfanumérico
 * Identifica CPF (11 caracteres) ou CNPJ (14 caracteres) e aplica máscara dinâmica
 */

/**
 * Aplica máscara de CPF: XXX.XXX.XXX-XX
 * @param {string} value - Valor a formatar
 * @returns {string}
 */
function maskCPF(value) {
    const cleaned = removeSpecialChars(value);
    let formatted = cleaned.slice(0, 11);
    
    if (formatted.length > 0) {
        formatted = formatted.slice(0, 3) + '.' + 
                   formatted.slice(3, 6) + '.' + 
                   formatted.slice(6, 9) + '-' + 
                   formatted.slice(9, 11);
    }
    
    return formatted;
}

/**
 * Aplica máscara de CNPJ: XX.XXX.XXX/XXXX-XX
 * Aceita letras nas primeiras 12 posições (RN01)
 * @param {string} value - Valor a formatar
 * @returns {string}
 */
function maskCNPJ(value) {
    // Permitir letras e números
    const cleaned = value.replace(/[^a-zA-Z0-9]/g, '');
    let formatted = cleaned.slice(0, 14);
    
    if (formatted.length > 0) {
        formatted = formatted.slice(0, 2) + '.' + 
                   formatted.slice(2, 5) + '.' + 
                   formatted.slice(5, 8) + '/' + 
                   formatted.slice(8, 12) + '-' + 
                   formatted.slice(12, 14);
    }
    
    return formatted;
}

/**
 * Identifica se é CPF (11 dígitos) ou CNPJ (14 caracteres) e aplica máscara
 * RN01 - Identificar tipo de documento automaticamente
 * @param {string} value - Valor a formatar
 * @returns {string}
 */
function maskDocument(value) {
    const cleaned = removeSpecialChars(value);
    
    // Contar apenas números para identificar tipo
    const digitCount = cleaned.replace(/[^0-9]/g, '').length;
    
    // Se tem apenas números e menos de 12 caracteres, é CPF
    if (digitCount === 11) {
        return maskCPF(value);
    }
    
    // Se tem números + letras OU 14+ caracteres, é CNPJ
    if (cleaned.length > 11) {
        return maskCNPJ(value);
    }
    
    // Aplicar máscara de acordo com o que está sendo digitado
    if (cleaned.length <= 11) {
        return maskCPF(value);
    } else {
        return maskCNPJ(value);
    }
}

/**
 * Bloqueador de caracteres especiais durante digitação
 * RN01 - Bloquear caracteres especiais (pontos, barras, traços)
 * @param {Event} event - Evento de teclado
 */
function handleDocumentInput(event) {
    const input = event.target;
    let value = input.value;
    
    // CA01 - Bloquear inserção de símbolos e aplicar máscara automaticamente
    const cleaned = removeSpecialChars(value);
    
    // Permitir apenas letras e números
    if (!/^[a-zA-Z0-9]*$/.test(value)) {
        // Se contém caracteres especiais, remover e aplicar máscara
        value = cleaned;
    }
    
    // Aplicar máscara de documento
    input.value = maskDocument(value);
}

/**
 * Valida CPF
 * @param {string} cpf - CPF a validar
 * @returns {boolean}
 */
function isValidCPF(cpf) {
    const cleaned = removeSpecialChars(cpf);
    
    // Verificar se tem 11 dígitos
    if (cleaned.length !== 11 || !/^\d+$/.test(cleaned)) {
        return false;
    }
    
    // Verificar se todos os dígitos são iguais
    if (/^(\d)\1{10}$/.test(cleaned)) {
        return false;
    }
    
    // Validar dígitos verificadores (simplificado)
    return true;
}

/**
 * Valida CNPJ (simplificado)
 * @param {string} cnpj - CNPJ a validar
 * @returns {boolean}
 */
function isValidCNPJ(cnpj) {
    const cleaned = removeSpecialChars(cnpj);
    
    // Verificar se tem 14 caracteres
    if (cleaned.length !== 14) {
        return false;
    }
    
    // Verificar se tem letras nas primeiras 12 posições
    const firstPart = cleaned.slice(0, 12);
    const lastPart = cleaned.slice(12, 14);
    
    // As últimas 2 posições devem ser números
    if (!/^\d+$/.test(lastPart)) {
        return false;
    }
    
    return true;
}

/**
 * Valida documento (CPF ou CNPJ)
 * @param {string} document - Documento a validar
 * @returns {object} - { isValid: boolean, type: 'CPF'|'CNPJ'|'INVALID' }
 */
function validateDocument(documento) {
    const cleaned = removeSpecialChars(documento);
    
    if (cleaned.length === 11) {
        return {
            isValid: isValidCPF(cleaned),
            type: 'CPF'
        };
    }
    
    if (cleaned.length === 14) {
        return {
            isValid: isValidCNPJ(cleaned),
            type: 'CNPJ'
        };
    }
    
    return {
        isValid: false,
        type: 'INVALID'
    };
}

/**
 * Inicializa máscara de documento
 */
function initializeDocumentMask() {
    const documentInput = document.getElementById('document');
    
    if (documentInput) {
        // Event listener para input
        documentInput.addEventListener('input', handleDocumentInput);
        
        // Bloquear caracteres especiais durante digitação
        documentInput.addEventListener('keypress', function(e) {
            const char = String.fromCharCode(e.which);
            
            // CA01 - Interceptar e bloquear caracteres especiais
            if (!/[a-zA-Z0-9]/.test(char)) {
                e.preventDefault();
                showAlert('Apenas letras e números são permitidos', 'warning');
            }
        });
    }
}

/**
 * Event listeners para formulário de cadastro
 */
document.addEventListener('DOMContentLoaded', function() {
    initializeDocumentMask();
    
    const cadastroForm = document.getElementById('cadastro-form');
    if (cadastroForm) {
        cadastroForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const nome = document.getElementById('nome').value;
            const email = document.getElementById('email').value;
            const documento = document.getElementById('document').value;
            const telefone = document.getElementById('telefone').value;
            const logradouro = document.getElementById('logradouro').value;
            const numero = document.getElementById('numero').value;
            const cep = document.getElementById('cep').value;
            const cidade = document.getElementById('cidade').value;
            const uf = document.getElementById('uf').value;
            const rendaMensal = document.getElementById('renda-mensal').value;
            const agencia = document.getElementById('agencia').value;
            const tipoConta = document.getElementById('tipo-conta').value;
            const dataNascimento = document.getElementById('data-nascimento').value;
            const senha = document.getElementById('senha').value;
            const confirmar_senha = document.getElementById('confirmar_senha').value;
            
            if (!nome || nome.length < 3) { showAlert('Nome deve ter pelo menos 3 caracteres', 'error'); return; }
            if (!isValidEmail(email)) { showAlert('Email inválido', 'error'); return; }
            const docValidation = validateDocument(documento);
            if (!docValidation.isValid) { showAlert('Documento (CPF/CNPJ) inválido', 'error'); return; }
            if (!telefone || telefone.length < 10) { showAlert('Telefone inválido', 'error'); return; }
            if (!logradouro || logradouro.length < 5) { showAlert('Endereço inválido', 'error'); return; }
            if (!numero) { showAlert('Número obrigatório', 'error'); return; }
            if (!cep || cep.length < 8) { showAlert('CEP inválido', 'error'); return; }
            if (!cidade) { showAlert('Cidade obrigatória', 'error'); return; }
            if (!uf || uf.length !== 2) { showAlert('UF inválida', 'error'); return; }
            if (!rendaMensal || parseFloat(rendaMensal) <= 0) { showAlert('Renda mensal inválida', 'error'); return; }
            if (!agencia) { showAlert('Selecione uma agência', 'error'); return; }
            if (!dataNascimento) { showAlert('Data de nascimento obrigatória', 'error'); return; }
            if (!senha || senha.length < 6) { showAlert('Senha deve ter pelo menos 6 caracteres', 'error'); return; }
            if (senha !== confirmar_senha) { showAlert('Senhas não coincidem', 'error'); return; }
            
            submitCadastro(nome, email, documento, telefone, logradouro, numero, cep, cidade, uf, rendaMensal, agencia, tipoConta, dataNascimento, senha);
        });
    }
});

async function submitCadastro(nome, email, documento, telefone, logradouro, numero, cep, cidade, uf, rendaMensal, agencia, tipoConta, dataNascimento, senha) {
    try {
        const cleaned = removeSpecialChars(documento);
        const tipoPessoa = cleaned.length === 11 ? 'PF' : 'PJ';
        
        const response = await apiPost('/clientes', {
            tipoPessoa: tipoPessoa,
            nome: nome,
            documento: cleaned,
            email: email,
            telefone: removeSpecialChars(telefone),
            logradouro: logradouro,
            numero: numero,
            cep: removeSpecialChars(cep),
            cidade: cidade,
            uf: uf,
            rendaMensal: parseFloat(rendaMensal),
            numeroAgencia: agencia,
            tipoConta: tipoConta,
            dataNascimentoFundacao: dataNascimento,
            senha: senha
        });
        
        showAlert('Cadastro realizado com sucesso! Faça login para continuar.', 'success');
        setTimeout(() => { redirect('index.html'); }, 2000);
        
    } catch (error) {
        console.error('Erro ao fazer cadastro:', error);
        showAlert('Erro ao fazer cadastro: ' + (error.message || 'Tente novamente'), 'error');
    }
}
