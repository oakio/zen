grammar Zen;

module
    : declaration+ EOF
    ;

expression
    : INTEGER                                           # IntegerLiteral
    | ('true'|'false')                                  # BoolLiteral
    | ID                                                # Id
    | call                                              # FunctionCall
    | '(' expression ')'                                # Parentheses
    | left=expression '&&' right=expression             # AndOperator
    | left=expression '||' right=expression             # OrOperator
    | left=expression op=('*'|'/'|'%') right=expression # Multiplication
    | left=expression op=('+'|'-') right=expression     # Addition
    | left=expression op=('<'|'<='|'>'|'>='|'=='|'!=') right=expression         # Relational
    | condition=expression '?' thenValue=expression ':' elseValue=expression    # Ternary
    ;

statement
    : return
    | call ';'
    | assign
    | varDeclare
    | ifElse
    | whileLoop
    | break
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

ifElse
    : 'if' '(' condition=expression ')' thenBlock=block ('else' elseBlock=block)?
    ;

whileLoop
    : 'while' '(' condition=expression ')' body=block
    ;

break
    : 'break' ';'
    ;

TYPE
    : 'i32'
    | 'void'
    | 'bool'
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