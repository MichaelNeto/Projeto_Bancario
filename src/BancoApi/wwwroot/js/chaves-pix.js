/**
 * Módulo para gerenciar Chaves Pix
 */

document.addEventListener('DOMContentLoaded', async function() {
    console.log('🔑 Inicializando página de Chaves Pix...');
    
    // Verificar autenticação
    initializeProtectedPage();
    preencherDadosLocalStorage();
    
    // Carregar dados do cliente
    await carregarDadosCliente();
    
    // Configurar abas
    configurarAbas();
    
    // Carregar chaves existentes
    await carregarChavesPix();
    
    // Configurar formulário
    configurarFormulario();
    
    // Configurar tipo de chave
    configurarTipoChave();
});

/**
 * Configura as abas de navegação
 */
function configurarAbas() {
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');
    
    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const tabName = btn.getAttribute('data-tab');
            
            // Remover active de todos
            tabBtns.forEach(b => b.classList.remove('active'));
            tabContents.forEach(tc => tc.classList.remove('show'));
            
            // Adicionar active ao clicado
            btn.classList.add('active');
            document.getElementById(tabName).classList.add('show');
        });
    });
}

/**
 * Carrega as chaves Pix do cliente
 */
async function carregarChavesPix() {
    try {
        const token = getToken();
        if (!token) {
            console.error('Token não disponível');
            return;
        }
        
        console.log('📥 Carregando chaves Pix...');
        const chaves = await apiGet('/pix/chaves', token);
        console.log('✅ Chaves carregadas:', chaves);
        
        renderizarChaves(chaves);
    } catch (error) {
        console.error('❌ Erro ao carregar chaves:', error);
        const lista = document.getElementById('chaves-list');
        if (lista) {
            lista.innerHTML = '<p class="no-transactions">Erro ao carregar chaves. Tente novamente.</p>';
        }
    }
}

/**
 * Renderiza a lista de chaves
 */
function renderizarChaves(chaves) {
    const lista = document.getElementById('chaves-list');
    if (!lista) return;
    
    lista.innerHTML = '';
    
    if (!chaves || chaves.length === 0) {
        lista.innerHTML = '<div class="empty-state"><p>Nenhuma chave Pix registrada ainda</p></div>';
        return;
    }
    
    chaves.forEach(chave => {
        const item = document.createElement('div');
        item.className = 'chave-item';
        
        const tipoEmoji = {
            'CPF': '👤',
            'EMAIL': '📧',
            'TELEFONE': '📱',
            'ALEATORIA': '🔀'
        };
        
        const dataFormatada = new Date(chave.dataCadastro).toLocaleDateString('pt-BR');
        
        item.innerHTML = `
            <div class="chave-info">
                <div class="chave-tipo">${tipoEmoji[chave.tipo] || '🔑'} ${chave.tipo}</div>
                <div class="chave-valor">${chave.valor}</div>
                <div class="chave-data">Registrada em ${dataFormatada}</div>
            </div>
            <button type="button" class="btn-delete" onclick="deletarChave('${chave.id}')">
                Remover
            </button>
        `;
        
        lista.appendChild(item);
    });
}

/**
 * Configura o comportamento do tipo de chave
 */
function configurarTipoChave() {
    const radios = document.querySelectorAll('input[name="tipo-chave"]');
    const inputGroups = {
        'CPF': 'input-cpf',
        'EMAIL': 'input-email',
        'TELEFONE': 'input-telefone',
        'ALEATORIA': 'input-aleatoria'
    };
    
    radios.forEach(radio => {
        radio.addEventListener('change', (e) => {
            // Ocultar todos
            Object.values(inputGroups).forEach(id => {
                const grupo = document.getElementById(id);
                if (grupo) grupo.classList.remove('show');
            });
            
            // Mostrar o selecionado
            const grupoId = inputGroups[e.target.value];
            if (grupoId) {
                document.getElementById(grupoId).classList.add('show');
            }
        });
    });
}

/**
 * Configura o formulário de nova chave
 */
function configurarFormulario() {
    const form = document.getElementById('nova-chave-form');
    if (!form) return;
    
    form.addEventListener('submit', handleNovaChave);
}

/**
 * Manipula o envio de nova chave Pix
 */
async function handleNovaChave(event) {
    event.preventDefault();
    
    const tipoRadio = document.querySelector('input[name="tipo-chave"]:checked');
    if (!tipoRadio) {
        showAlert('Selecione um tipo de chave', 'error');
        return;
    }
    
    const tipo = tipoRadio.value;
    let valor = '';
    
    // Obter valor conforme o tipo
    if (tipo === 'CPF') {
        valor = document.getElementById('valor-cpf').value.trim();
        if (!valor) {
            showAlert('Digite o CPF', 'error');
            return;
        }
    } else if (tipo === 'EMAIL') {
        valor = document.getElementById('valor-email').value.trim();
        if (!valor) {
            showAlert('Digite o e-mail', 'error');
            return;
        }
        if (!isValidEmail(valor)) {
            showAlert('E-mail inválido', 'error');
            return;
        }
    } else if (tipo === 'TELEFONE') {
        valor = document.getElementById('valor-telefone').value.trim();
        if (!valor) {
            showAlert('Digite o telefone', 'error');
            return;
        }
    } else if (tipo === 'ALEATORIA') {
        valor = ''; // Será gerado no backend
    }
    
    // Desabilitar botão
    const submitBtn = event.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = 'Processando...';
    
    try {
        const token = getToken();
        if (!token) {
            showAlert('Token não encontrado', 'error');
            submitBtn.disabled = false;
            submitBtn.textContent = originalText;
            return;
        }
        
        const dados = {
            tipo: tipo,
            valor: valor || null
        };
        
        console.log('📤 Cadastrando chave Pix:', dados);
        const response = await apiPost('/pix/chaves', dados, token);
        console.log('✅ Resposta:', response);
        
        showAlert('Chave Pix cadastrada com sucesso!', 'success');
        
        // Limpar formulário
        event.target.reset();
        document.querySelectorAll('.input-group').forEach(ig => ig.classList.remove('show'));
        
        // Recarregar chaves
        setTimeout(() => {
            carregarChavesPix();
            
            // Voltar para aba de chaves
            document.querySelector('[data-tab="minhas-chaves"]').click();
        }, 1000);
        
    } catch (error) {
        console.error('❌ Erro ao cadastrar chave:', error);
        const mensagem = error.message || 'Erro ao cadastrar chave Pix';
        showAlert(mensagem, 'error');
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = originalText;
    }
}

/**
 * Deleta uma chave Pix
 */
async function deletarChave(id) {
    if (!confirm('Tem certeza que deseja remover esta chave Pix?')) {
        return;
    }
    
    try {
        const token = getToken();
        if (!token) {
            showAlert('Token não encontrado', 'error');
            return;
        }
        
        console.log('🗑️  Deletando chave:', id);
        
        const response = await fetch(await detectApiBaseUrl() + `/pix/chaves/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao remover chave');
        }
        
        showAlert('Chave Pix removida com sucesso', 'success');
        carregarChavesPix();
        
    } catch (error) {
        console.error('❌ Erro ao deletar chave:', error);
        showAlert(error.message || 'Erro ao remover chave Pix', 'error');
    }
}

/**
 * Valida formato de email
 */
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

/**
 * Exibe alerta na tela
 */
function showAlert(titulo, tipo = 'info') {
    // Usar função global do auth.js se disponível
    if (typeof showNotification === 'function') {
        showNotification('Chaves Pix', titulo, tipo);
    } else {
        alert(titulo);
    }
}

/**
 * Alternativa de showAlert usando notificação nativa
 */
function showNotification(titulo, mensagem, tipo = 'info') {
    let container = document.getElementById('notification-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'notification-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            display: flex;
            flex-direction: column;
            gap: 10px;
            max-width: 400px;
        `;
        document.body.appendChild(container);
    }

    const notification = document.createElement('div');
    const bgColor = {
        'success': '#10b981',
        'error': '#ef4444',
        'warning': '#f59e0b',
        'info': '#3b82f6'
    }[tipo] || '#3b82f6';

    notification.style.cssText = `
        background-color: ${bgColor};
        color: white;
        padding: 16px;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        font-weight: 500;
        animation: slideIn 0.3s ease-out;
    `;
        
    const tituloDiv = document.createElement('div');
    tituloDiv.style.cssText = 'font-weight: 600; margin-bottom: 4px;';
    tituloDiv.textContent = titulo;

    const mensagemDiv = document.createElement('div');
    mensagemDiv.style.cssText = 'font-size: 14px;';
    mensagemDiv.textContent = mensagem;

    notification.appendChild(tituloDiv);
    notification.appendChild(mensagemDiv);

    container.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease-out forwards';
        setTimeout(() => notification.remove(), 300);
    }, 5000);
}
