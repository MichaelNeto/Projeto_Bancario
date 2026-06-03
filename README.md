#Core Banking Platform — Event-Driven and High Performance

Sistema de Core Banking desenvolvido com foco em alta concorrência, resiliência e processamento de eventos em tempo real, utilizando .NET, Kafka e PostgreSQL.

Demonstração
Execução do sistema
./docs/demo.gif

Vídeo completo
 
## 🎥 Vídeo Completo

[▶ Assistir Demonstração](https://youtu.be/d6qI07w7oBQ?si=XaxbrUWuYMJvLyyN)

#Sobre
A plataforma implementa uma arquitetura orientada a eventos (Event-Driven Architecture), processando operações como Pix, transações financeiras e crédito de forma assíncrona, escalável e desacoplada.

O sistema foi projetado para simular condições reais de produção, incluindo cenários de alta concorrência e processamento distribuído.

#Arquitetura
Clean Architecture em .NET (C#)

Microserviços (C# e Java)

Comunicação assíncrona via Apache Kafka

Processamento orientado a eventos (EDA)

Infraestrutura containerizada com Docker

#Diferencial Técnico
O projeto incorpora UVM (Universal Verification Methodology) como estratégia avançada de simulação de cenários.

Essa abordagem permite:

geração de eventos simultâneos e não determinísticos

simulação de latência e falhas

testes de consistência em operações como Pix

validação de idempotência

identificação de gargalos de performance

Com isso, o sistema é validado sob condições reais de carga, indo além de testes convencionais.

Stack Tecnológica
.NET (C#)

Java

Apache Kafka

PostgreSQL

Docker e Docker Compose

JWT (autenticação)

BCrypt (criptografia de senha)

JavaScript (frontend)

#Estratégia de Testes
O projeto adota uma abordagem completa:

Testes unitários (xUnit, JUnit)

Testes de integração (Testcontainers com Kafka e PostgreSQL reais)

Testes end-to-end (fluxos completos de Pix e transações)

Testes de contrato (eventos Kafka)

Testes de concorrência e stress com UVM

Execução
Shell

docker-compose up -d

dotnet test

Mostrar mais linhas

Estrutura do Projeto
Shell

/src

 /core-banking-api

 /microservices

 /event-consumers

/tests

 /unit

 /integration

 /e2e

/docker

/docs

 demo.gif

Mostrar mais linhas

Roadmap
Evolução dos testes de stress com UVM

Implementação de tracing distribuído

Otimizações de performance no Kafka

Estratégias de retry e Dead Letter Queue

Expansão de microserviços

Autor
Michael Silva
Engenharia de Software | Microeletrônica | Sistemas Distribuídos
