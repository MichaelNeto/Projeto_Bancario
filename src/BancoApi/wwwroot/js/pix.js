/**
 * RN03 - Envio Autorizado de Requisições
 * RN04 - Renderização Dinâmica do Extrato
 * Responsável por PIX e requisições autorizadas com JWT
 */

/**
 * Carrega o saldo da conta
 * O saldo não tem endpoint específico - será obtido do extrato
 */
async function loadBalance() {
    try {
        // Para obter o saldo, podemos usar o extrato ou criar um endpoint específico
        // Por enquanto, vamos deixar este placeholder
        console.log('Saldo será carregado junto com o extrato');
    } catch (error) {
        console.error('Erro ao carregar saldo:', error);
    }
}

/**
 * Carrega o extrato da conta
 * RN03 - Incluir Token JWT no cabeçalho
 * RN04 - Processar array de transações com classes CSS dinâmicas
 */
async function loadExtrato() {
    try {
        const token = getToken();
        
        if (!token) {
            showAlert('Sessão expirada. Faça login novamente.', 'error');
            redirect('index.html');
            return;
        }
        
        // RN03 - Enviar requisição autorizada com JWT
        console.log('🔄 Iniciando carregamento do extrato...');
        const response = await apiGet('/extrato', token);
        console.log('✅ Resposta do extrato recebida:', response);
        
        let transacoes = [];
        
        // Tentar extrair array de transações de diferentes formatos
        if (Array.isArray(response)) {
            console.log('ℹ️ Resposta é um array direto');
            transacoes = response;
        } else if (response && response.transacoes && Array.isArray(response.transacoes)) {
            console.log('ℹ️ Resposta tem propriedade transacoes');
            transacoes = response.transacoes;
        } else if (response && typeof response === 'object') {
            console.log('ℹ️ Resposta é um objeto, tentando converter...');
            // Se for um objeto único, tentar colocar em array
            transacoes = [response];
        } else {
            console.log('⚠️ Resposta vazia ou formato desconhecido');
            transacoes = [];
        }
        
        console.log('📊 Total de transações a renderizar:', transacoes.length);
        renderExtrato(transacoes);
        return transacoes;
        
    } catch (error) {
        console.error('❌ Erro ao carregar extrato:', error);
        
        // Mostrar mensagem de erro no container
        const extratoContainer = document.getElementById('extrato-list');
        if (extratoContainer) {
            extratoContainer.innerHTML = `<p class="no-transactions">Erro ao carregar extrato. Tente novamente.</p>`;
        }
        
        showAlert('Erro ao carregar extrato: ' + (error.message || 'Tente novamente'), 'error');
    }
}

/**
 * RN04 - Renderiza o extrato dinamicamente com classes CSS
 * Aplica classes baseadas no tipo e status da transação
 * @param {Array} transacoes - Array de transações da API
 */
function renderExtrato(transacoes) {
    const extratoContainer = document.getElementById('extrato-list');
    
    if (!extratoContainer) {
        console.error('❌ Elemento extrato-list não encontrado no DOM');
        return;
    }
    
    console.log(`📋 Renderizando extrato com ${transacoes ? transacoes.length : 0} transações`);
    
    // Limpar conteúdo anterior
    extratoContainer.innerHTML = '';
    
    // Validar se é um array
    if (!Array.isArray(transacoes)) {
        console.error('❌ Transações não é um array:', transacoes);
        extratoContainer.innerHTML = '<p class="no-transactions">Erro: formato de dados inválido</p>';
        return;
    }
    
    if (transacoes.length === 0) {
        console.log('ℹ️ Nenhuma transação encontrada');
        extratoContainer.innerHTML = '<p class="no-transactions">Nenhuma transação encontrada</p>';
        return;
    }
    
    console.log(`✅ Iniciando renderização de ${transacoes.length} transações`);
    
    transacoes.forEach((transacao, index) => {
        try {
            console.log(`  📌 Processando transação ${index + 1}/${transacoes.length}:`, transacao);
            const item = document.createElement('div');
            item.className = 'extrato-item';
            
            // Determinar classe CSS baseado no tipo e status (RN04)
            let classe = '';
            let sinal = '';
            let statusClass = '';
            
            if (transacao.status === 'Falhou' || transacao.status === 'FALHOU') {
                // RN04 - Status FALHOU: classe 'estornado'
                classe = 'estornado';
                statusClass = 'status-failed';
            } else if (transacao.tipo === 'PIX_RECEBIDO' || transacao.tipo === 'DEPOSITO_RECEBIDO') {
                // RN04 - PIX_RECEBIDO e DEPOSITO_RECEBIDO: classe 'ganho' (verde com +)
                classe = 'ganho';
                sinal = '+';
            } else if (transacao.tipo === 'PIX_ENVIADO') {
                // RN04 - PIX_ENVIADO: classe 'gasto' (vermelho com -)
                classe = 'gasto';
                sinal = '-';
            }
            
            item.classList.add(classe);
            
            // Construir HTML da transação
            let html = `
                <div class="extrato-header">
                    <div class="extrato-info">
                        <p class="extrato-tipo">${getTipoLabel(transacao.tipo)}</p>
                        <p class="extrato-data">${formatDate(transacao.dataHora)}</p>
                    </div>
                    <div class="extrato-valor ${statusClass}">
                        ${sinal} ${formatCurrency(Math.abs(transacao.valor))}
                    </div>
                </div>
            `;
            
            // Se status for FALHOU, exibir motivo
            if (transacao.status === 'FALHOU' && transacao.motivo_falha) {
                html += `<p class="extrato-motivo">❌ ${transacao.motivo_falha}</p>`;
            }
            
            // Exibir dados adicionais do PIX
            if (transacao.tipo === 'PIX_RECEBIDO' || transacao.tipo === 'PIX_ENVIADO') {
                html += `<p class="extrato-descricao">${transacao.nomeContraparte || transacao.descricao || ''}</p>`;
            } else if (transacao.tipo === 'DEPOSITO_RECEBIDO') {
                html += `<p class="extrato-descricao">${transacao.observacao || 'Depósito em conta'}</p>`;
            }
            
            item.innerHTML = html;
            extratoContainer.appendChild(item);
            console.log(`    ✓ Transação ${index + 1} renderizada`);
        } catch (itemError) {
            console.error(`❌ Erro ao processar transação ${index + 1}:`, itemError, transacao);
        }
    });
    
    console.log(`✅ Extrato completamente renderizado. Total de ${transacoes.length} itens`);
}

/**
 * Obtém rótulo legível para tipo de transação
 * @param {string} tipo - Tipo da transação
 * @returns {string}
 */
function getTipoLabel(tipo) {
    const labels = {
        'PIX_RECEBIDO': '📥 PIX Recebido',
        'PIX_ENVIADO': '📤 PIX Enviado',
        'DEPOSITO_RECEBIDO': '💰 Depósito',
        'TRANSFERENCIA': '💳 Transferência',
        'DEPOSITO': '💰 Depósito',
        'SAQUE': '💵 Saque'
    };
    
    return labels[tipo] || tipo;
}

/**
 * Envia um PIX
 * RN03 - Incluir Token JWT no cabeçalho
 * @param {string} chave_pix - Chave PIX do destinatário
 * @param {number} valor - Valor do PIX
 */
async function enviarPix(chave_pix, valor) {
    try {
        const token = getToken();
        
        if (!token) {
            showAlert('Sessão expirada. Faça login novamente.', 'error');
            redirect('index.html');
            return;
        }
        
        if (!chave_pix || chave_pix.trim() === '') {
            showAlert('Chave PIX inválida', 'error');
            return;
        }
        
        if (!valor || valor <= 0) {
            showAlert('Valor deve ser maior que zero', 'error');
            return;
        }
        
        // RN03 - Enviar requisição autorizada com JWT no cabeçalho
        const response = await apiPost('/pix/transferir', {
            chaveDestino: chave_pix,
            valor: parseFloat(valor)
        }, token);
        
        showAlert('PIX enviado com sucesso!', 'success');
        
        // Recarregar extrato
        setTimeout(() => {
            loadExtrato();
        }, 1000);
        
        return response;
    } catch (error) {
        console.error('Erro ao enviar PIX:', error);
        showAlert('Erro ao enviar PIX: ' + (error.message || 'Tente novamente'), 'error');
    }
}

/**
 * Valida chave PIX
 * @param {string} chave - Chave PIX a validar
 * @returns {boolean}
 */
function isValidPixKey(chave) {
    // Email
    if (/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(chave)) {
        return true;
    }
    
    // CPF
    if (/^\d{11}$/.test(chave)) {
        return true;
    }
    
    // CNPJ
    if (/^\d{14}$/.test(chave)) {
        return true;
    }
    
    // Telefone
    if (/^\d{11}$/.test(chave)) {
        return true;
    }
    
    // Chave aleatória
    if (/^[a-f0-9\-]{36}$/.test(chave)) {
        return true;
    }
    
    return false;
}

/**
 * Event listeners para formulário de PIX
 */
document.addEventListener('DOMContentLoaded', async function() {
    // Carregar dados ao abrir a página
    const dashboardElement = document.getElementById('dashboard');
    if (dashboardElement) {
        console.log('🚀 Dashboard encontrado, inicializando...');
        initializeProtectedPage();
        
        // Preencher dados iniciais do localStorage (fallback)
        preencherDadosLocalStorage();
        
        // Carregar dados da API (aguardar conclusão)
        console.log('👤 Carregando dados do cliente...');
        await carregarDadosCliente();
        
        loadBalance();
        
        console.log('📋 Carregando extrato...');
        try {
            await loadExtrato();
            console.log('✅ Extrato carregado com sucesso');
        } catch (error) {
            console.error('❌ Erro crítico ao carregar extrato:', error);
        }
        
        // Atualizar a cada 30 segundos
        setInterval(() => {
            console.log('🔄 Atualizando dados (intervalo 30s)...');
            carregarDadosCliente();  // Atualiza saldo em tempo real
            loadBalance();
            loadExtrato();
        }, 30000);
    } else {
        console.warn('⚠️ Dashboard não encontrado');
    }
        },);
    
    
    // Verificar se há formulário de PIX
    const pixForm = document.getElementById('pix-form');
    if (pixForm) {
        pixForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const chave_pix = document.getElementById('chave-pix').value;
            const valor = document.getElementById('valor-pix').value;
            
            if (!isValidPixKey(chave_pix)) {
                showAlert('Chave PIX inválida', 'error');
                return;
            }
            
            enviarPix(chave_pix, valor).then(() => {
                // Limpar formulário
                pixForm.reset();
            });
        });
    }
