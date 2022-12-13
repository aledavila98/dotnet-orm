using ORM.Core.Enums;
using ORM.Core.Interfaces;
using ORM.Core.Models;

namespace ORM.Parser
{
    public class Parser : IParser
    {
        private readonly IScanner _scanner;
        private readonly ILogger _logger;
        private Token _lookAhead;

        public Parser(IScanner scanner, ILogger logger)
        {
            this._scanner = scanner;
            this._logger = logger;
            this._lookAhead = this._scanner.GetNextToken();
        }

        public void Parse()
        {
            Stmts();
        }
        private void Stmts()
        {
            if (this._lookAhead == TokenType.OpenBrace)
            {
                Stmt();
                Stmts();
            }
        }

        private void Stmt()
        {
            Match(TokenType.Identifier);
            Match(TokenType.Dot);
            if (this._lookAhead.TokenType == TokenType.WhereKeyword)
            {
                WhereStmt();
            } else
            {
                SelectStmt();
            }
        }

        private void WhereStmt()
        {
            Match(TokenType.WhereKeyword);
            Match(TokenType.LeftParens);
            Match(TokenType.Identifier);
            Match(TokenType.Arrow);
            WhereExpr();
            Match(TokenType.RightParens);
        }

        private void SelectStmt()
        {
            Match(TokenType.SelectKeyword);
            Match(TokenType.LeftParens);
            Match(TokenType.Identifier);
            Match(TokenType.Arrow);
            WhereExpr();
            Match(TokenType.RightParens);
        }

        private void WhereExpr()
        {
            if (this._lookAhead.TokenType != TokenType.RightParens)
            {
                switch (this._lookAhead.TokenType)
                {
                    case TokenType.Identifier:
                        Match(TokenType.Identifier);
                        Match(TokenType.Dot);
                        Match(TokenType.Identifier);
                        BoolExpr();
                        break;
                    case TokenType.IntConstant:
                        Match(TokenType.IntConstant);
                        break;
                    case TokenType.FloatConstant:
                        Match(TokenType.FloatConstant);
                        break;
                    case TokenType.StringConstant:
                        Match(TokenType.StringConstant);
                        break;
                }
                WhereExpr();
            }
        }

        private void BoolExpr()
        {
            switch (this._lookAhead.TokenType)
            {
                case TokenType.GreaterThan:
                    Match(TokenType.GreaterThan);
                    if (_lookAhead.TokenType == TokenType.Equal)
                    {
                        Match(TokenType.Equal);
                    }
                    break;
                case TokenType.LessThan:
                    Match(TokenType.LessThan);
                    if (_lookAhead.TokenType == TokenType.Equal)
                    {
                        Match(TokenType.Equal);
                    }
                    break;
                case TokenType.NotEqual:
                    Match(TokenType.NotEqual);
                    break;
                case TokenType.EqualOperator:
                    Match(TokenType.EqualOperator);
                    break;
                case TokenType.Or:
                    Match(TokenType.Or);
                    break;
                case TokenType.And:
                    Match(TokenType.And);
                    break;
            }
        }
        private void Move()
        {
            this._lookAhead = this._scanner.GetNextToken();
        }

        private void PClass()
        {
            Match(TokenType.ClassKeyword);
            Match(TokenType.Identifier);
            Match(TokenType.OpenBrace);
            Match(TokenType.OpenBrace);
            ClassBlock();
            Match(TokenType.CloseBrace);
        }

        private void ClassBlock()
        {
            if (this._lookAhead.TokenType != TokenType.CloseBrace)
            {
                switch(this._lookAhead.TokenType)
                {
                    case TokenType.StringKeyword:
                        Match(TokenType.StringKeyword);
                        break;
                    case TokenType.IntKeyword:
                        Match(TokenType.IntKeyword);
                        break;
                    case TokenType.FloatKeyword:
                        Match(TokenType.FloatKeyword);
                        break;
                    case TokenType.Identifier:
                        Match(TokenType.Identifier);
                        Match(TokenType.SemiColon);
                        break;
                    default:
                        throw new Exception();
                }
                ClassBlock();
            }
        }

        private void Match(TokenType expectedTokenType)
        {
            if (this._lookAhead != expectedTokenType)
            {
                this._logger.Error($"Syntax Error! expected token {expectedTokenType} but found {this._lookAhead.TokenType} on line {this._lookAhead.Line} and column {this._lookAhead.Column}");
                throw new ApplicationException($"Syntax Error! expected token {expectedTokenType} but found {this._lookAhead.TokenType} on line {this._lookAhead.Line} and column {this._lookAhead.Column}");
            }
            this.Move();
        }
    }
}