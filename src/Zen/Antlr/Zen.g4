grammar Zen;

module
    : declaration+ EOF
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
    : '{' '}'
    ;

TYPE
    : 'i32'
    | 'void'
    ;

ID
    : [a-zA-Z_]+[0-9a-zA-Z_]*
    ;

WS
    : [ \r\n\t] -> skip
    ;