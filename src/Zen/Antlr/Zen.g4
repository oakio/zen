grammar Zen;

module
    : declaration+ EOF
    ;

expression
    : INTEGER                                           # IntegerLiteral
    | ID                                                # Id
    ;

statement
    : return
    ;

declaration
    : funcDeclare
    ;

funcDeclare
    : TYPE ID '(' param? (',' param)* ')' ';'
    | TYPE ID '(' param? (',' param)* ')' block?
    ;

param
    : TYPE ID
    ;

block
    : '{' statement* '}'
    ;

return
    : 'return' expression? ';'
    ;

TYPE
    : 'i32'
    | 'void'
    ;

ID
    : [a-zA-Z_]+[0-9a-zA-Z_]*
    ;

INTEGER
    : [0-9]+
    ;

WS
    : [ \r\n\t] -> skip
    ;