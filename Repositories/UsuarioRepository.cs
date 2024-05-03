﻿using Dapper;
using JogadorAPI.DTO;
using JogadorAPI.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Opw.HttpExceptions;

namespace JogadorAPI.Repositories
{
    public class UsuarioRepository : Repository<Usuario>
    {
        private readonly MySqlConnection _connection;

        public UsuarioRepository(MySqlConnection connection) : base(connection)
            => _connection = connection;

        public dynamic Create(Usuario usuario)
        {
            string sql = @"INSERT INTO Usuario 
                               (EMAIL, TELEFONE, CPF, NOME, SENHA, CIDADE, BAIRRO) 
                           VALUES 
                               (@Email, @Telefone, @CPF, @Nome, @Senha, @Cidade, @Bairro)";

            var data = new
            {
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                CPF = usuario.CPF,
                Nome = usuario.Nome,
                Senha = usuario.Senha,
                Cidade = usuario.Cidade,
                Bairro = usuario.Bairro
            };

            var rowsAffected = _connection.Execute(sql, data);
            return rowsAffected;
        }

        public string GetPasswordByEmail(string email)
        {
            var query = @"SELECT SENHA FROM Usuario WHERE Usuario.EMAIL = @Email";

            var hashSenha = _connection.Query<string>(query, new { Email = email });

            if (hashSenha.FirstOrDefault() == null)
                throw new BadRequestException("Email ou senha incorretos");

            return hashSenha.FirstOrDefault();
        }

        public LoginSessionDTO GetSessionLoginByEmail(string email)
        {
            var query = @"SELECT ID_USUARIO, EMAIL FROM Usuario WHERE Usuario.EMAIL = @Email";

            var loginSession = _connection.Query<LoginSessionDTO>(query, new { Email = email });
            return loginSession.FirstOrDefault();
        }
    }
}
