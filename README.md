# Get started
Neste tópico será listado algumas informações importantes sobre o projeto.


## Sobre o projeto
O projeto é um **API REST** desenvolvido em  .net core 3.1, utilizando o padrão de modelagem `Domain Driven Design` e tem como foco principal, autenticar usuários e gerenciar níveis de acesso a diferentes módulos do sistema, auditando todos os dados transacionais.

O projeto possui a documentação da `API` gerada pelo **Swagger.**  

## Documentação
Os documentos abaixo, encontram-se na pasta `documentações`, deste repositório.
 
 - Diagrama de processo
 - Diagrama de classes
 - diagrama de banco de dados

A documentação da `API` é o próprio **Swagger**.

## Dependências
Todas as dependências e sua versão utilizada neste projeto, está contido na lista abaixo: 
 - Microsoft.AspNetCore.Authentication.JwtBearer 3.1.8
 - Microsoft.AspNetCore.Mvc.NewtonsoftJson 3.1.8
 - Microsoft.EntityFrameworkCore.SqlServe 3.1.8
 - Newtonsoft.Json 12.0.3
 - Serilog.Extensions.Logging.File 2.0.0
 - Swashbuckle.AspNetCore 5.5.1
 - Swashbuckle.AspNetCore.Newtonsoft 5.5.1
 - Swashbuckle.AspNetCore.Swagger 5.5.1
 - Swashbuckle.AspNetCore.SwaggerGen 5.5.1
 - Swashbuckle.AspNetCore.SwaggerUi 5.5.1
 - Microsoft.AspNetCore.Identity.EntityFrameworkCore 3.1.8
 - Microsoft.EntityFrameworkCore 3.1.8
 - Microsoft.EntityFrameworkCore.SqlServer 3.1.8
 - Microsoft.EntityFrameworkCore.Tools 3.1.8

## Autenticação
A autenticação do usuário foi realizada utilizando o `Identity` do .net core 3.1. Esta API controla o gerenciamento de usuário, perfis e afirmações (claim). Usando o `identity`é possível salvar usuários no banco de dados sem identificar o password devido ao fato que o password é salvo com uma criptografia do próprio `Identity`.

Quando o usuário é autenticado no sistema, um `jason web token` é gerado. Este token possui afirmações sobre o usuário, como perfil, permissões e login. 

## Autorização

A cada requisição para um `endpoint` é verificado se o usuário possui a permissão para  executar a ação. Estas permissões foram dadas através de perfil.  Cada perfil possui autorizações independentes, criando a possibilidade inúmeros perfis com permissões diferente.

Estas permissões se dividem em 3 grupos, são eles: **Manager, Viwer e user.**

 - **Manager**: Fornece acesso para visualizar e modificar
   itens de módulo especifico. 
 - **Viwer**: Fornece acesso
   apenas a visualização de todos os itens de um determinado módulo. 
 - **User**: É a permissão default para todos os perfis e libera
   acesso somente aos itens do usuário logado.

## Auditoria
Toda requisição feita para `API` gera um log, que foi divido em dois níveis: 

 - Informação
 - Error

Os logs de informação estão atrelados a ações executadas pelo usuário e os logs de erro está ligado a exceções lançadas pelo sistema.

## Arquitetura

O projeto utiliza o modelo organizacional DDD(Domain, Driven, Design) e utiliza o padrão de arquitetura Repository, isolando o acesso aos dados seguindo o principio SOLID.

## Criação de Módulos Adicionais
Como o propósito deste desenvolvimento é gerenciar os acessos e permissões foram inserido dois módulos para exemplificar. Assim como estes dois módulos, o sistema é capaz de ter inúmeros módulos cada qual com seu grupo de permissão.

