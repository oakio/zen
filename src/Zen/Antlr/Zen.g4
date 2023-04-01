grammar Zen;

module
    : declaration+ EOF
    ;

expression
    : INTEGER                                           # IntegerLiteral
    | ID                                                # Id
    | call                                              # FunctionCall
    | '(' expression ')'                                # Parentheses
    | left=expression op=('*'|'/'|'%') right=expression # Multiplication
    | left=expression op=('+'|'-') right=expression     # Addition
    ;

statement
    : return
    | call ';'
    | assign
    | varDeclare
    ;

declaration
    : funcDeclare
    ;

varDeclare
    : TYPE ID '=' expression ';';

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

call
    : ID '(' expression? (',' expression)* ')'
    ;

assign
    : ID '=' value=expression ';'
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

COMMENT
    : '//' ~[\r\n]* -> skip
    ;