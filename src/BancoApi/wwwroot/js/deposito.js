/**
 * Módulo para gerenciar depósitos em conta
 */

document.addEventListener('DOMContentLoaded', () => {
    const depositoForm = document.getElementById('deposito-form');
    
    if (depositoForm) {
        depositoForm.addEventListener('submit', handleDepositoSubmit);
    }
});

/**
 * Manipula o envio do formulário de depósito
 * @param {Event} event - Evento do formulário
 */
async function handleDepositoSubmit(event) {
    event.preventDefault();

    const valorInput = document.getElementById('valor-deposito');
    const valor = parseFloat(valorInput.value);

    // Validar valor
    if (isNaN(valor) || valor <= 0) {
        showNotification('Erro', 'Digite um valor válido maior que zero', 'error');
        return;
    }

    // Desabilitar botão durante o envio
    const submitBtn = event.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.textContent;
    submitBtn.disabled = true;
    submitBtn.textContent = 'Processando...';

    try {
        const token = getToken();
        if (!token) {
            showNotification('Erro', 'Token não encontrado. Faça login novamente', 'error');
            submitBtn.disabled = false;
            submitBtn.textContent = originalText;
            return;
        }

        const depositoRequest = {
            valor: valor
        };

        const response = await apiPost('/transacoes/deposito', depositoRequest, token);

        // Sucesso
        showNotification('Sucesso', `Depósito de ${formatCurrency(valor)} realizado com sucesso!`, 'success');
        
        // Atualizar saldo imediatamente com o valor retornado da API
        if (response.saldoAtual !== undefined) {
            const saldoElement = document.getElementById('saldo');
            if (saldoElement) {
                saldoElement.textContent = formatCurrency(response.saldoAtual);
                console.log('Saldo atualizado imediatamente:', formatCurrency(response.saldoAtual));
            }
        }
        
        // Limpar formulário
        event.target.reset();
        valorInput.focus();

        // Recarregar dados completos do cliente após breve delay
        setTimeout(() => {
            carregarDadosCliente();
            
            // Recarregar o extrato se existir
            if (typeof loadExtrato === 'function') {
                loadExtrato();
            }
        }, 500);

    } catch (error) {
        console.error('Erro ao realizar depósito:', error);
        const mensagem = error.message || 'Erro ao processar o depósito';
        showNotification('Erro', mensagem, 'error');
    } finally {
        submitBtn.disabled = false;
        submitBtn.textContent = originalText;
    }
}

/**
 * Exibe notificação na tela
 * @param {string} titulo - Título da notificação
 * @param {string} mensagem - Mensagem da notificação
 * @param {string} tipo - Tipo: 'success', 'error', 'warning', 'info'
 */
function showNotification(titulo, mensagem, tipo = 'info') {
    // Criar container de notificação se não existir
    let notificationContainer = document.getElementById('notification-container');
    if (!notificationContainer) {
        notificationContainer = document.createElement('div');
        notificationContainer.id = 'notification-container';
        notificationContainer.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            display: flex;
            flex-direction: column;
            gap: 10px;
            max-width: 400px;
        `;
        document.body.appendChild(notificationContainer);
    }

    // Criar elemento de notificação
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

    notification.innerHTML = `
        <div style="font-weight: 600; margin-bottom: 4px;">${titulo}</div>
        <div style="font-size: 14px;">${mensagem}</div>
    `;

    notificationContainer.appendChild(notification);

    // Remover automaticamente após 5 segundos
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease-out forwards';
        setTimeout(() => notification.remove(), 300);
    }, 5000);
}

// Adicionar animações CSS
if (!document.getElementById('notification-styles')) {
    const style = document.createElement('style');
    style.id = 'notification-styles';
    style.textContent = `
        @keyframes slideIn {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(400px);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(style);
}
