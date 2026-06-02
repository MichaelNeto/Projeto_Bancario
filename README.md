#Core Banking Platform — Event-Driven and High Performance

Este projeto implementa um Core Banking moderno baseado em arquitetura orientada a eventos (Event-Driven Architecture), com foco em escalabilidade, resiliência e alta concorrência.
A solução foi desenvolvida utilizando .NET (C#), com integração entre microserviços (C# e Java), comunicação assíncrona via Apache Kafka e persistência em PostgreSQL, executando em ambiente containerizado com Docker.
O sistema suporta operações financeiras essenciais como cadastro de clientes, autenticação, transferências Pix, processamento de transações e concessão de crédito.

#Arquitetura
A aplicação segue os princípios de Clean Architecture, promovendo separação de responsabilidades e fácil evolução do código.
Características principais:

Microserviços desacoplados
Comunicação baseada em eventos com Kafka
Processamento assíncrono
Infraestrutura em Docker


#Diferencial Técnico
O projeto incorpora UVM (Universal Verification Methodology) como estratégia avançada de simulação de cenários de alta concorrência.
Essa abordagem permite:

geração de eventos simultâneos e não determinísticos
simulação de latência e falhas
validação de idempotência
testes realistas de carga em operações como Pix

Com isso, o sistema é validado em condições mais próximas de produção, indo além dos testes tradicionais.

#Stack Tecnológica

.NET (C#)
Java
Apache Kafka
PostgreSQL
Docker e Docker Compose
JWT para autenticação
BCrypt para criptografia de senhas
JavaScript no frontend


#Estratégia de Testes
O projeto utiliza múltiplas camadas de testes:

Testes unitários para regras de negócio
Testes de integração com Testcontainers (Kafka e PostgreSQL reais)
Testes end-to-end para validação completa dos fluxos
Testes de concorrência e stress utilizando UVM


Objetivo
Construir um sistema financeiro preparado para operar sob condições reais de carga, garantindo consistência, resiliência e previsibilidade.
