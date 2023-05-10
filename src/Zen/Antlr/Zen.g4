grammar Zen;

module
    : declaration+ EOF
    ;

expression
    : INTEGER                                           # IntegerLiteral
    | FLOAT                                             # FloatLiteral
    | ('true'|'false')                                  # BoolLiteral
    | ID                                                # Id
    | call                                              # FunctionCall
    | '(' expression ')'                                # Parentheses
    | '(' type ')' expression                           # Casting
    | op=('-'|'!') expression                           # Unary
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
    | continue
    ;

declaration
    : funcDeclare
    ;

varDeclare
    : type ID '=' expression ';';

funcDeclare
    : type ID '(' param? (',' param)* ')' ';'
    | type ID '(' param? (',' param)* ')' block?
    ;

param
    : type ID
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

continue
    : 'continue' ';'
    ;

type
    : BUILTINTYPE   # BuiltinType
    ;

BUILTINTYPE
    : 'void'
    | 'bool'
    | 'i8'
    | 'u8'
    | 'i16'
    | 'u16'
    | 'i32'
    | 'u32'
    | 'i64'
    | 'u64'
    | 'f32'
    | 'f64'
    ;

ID
    : [a-zA-Z_]+[0-9a-zA-Z_]*
    ;

INTEGER
    : [0-9]+
    ;

FLOAT
    : [0-9]+'.'[0-9]+
    ;

WS
    : [ \r\n\t] -> skip
    ;

COMMENT
    : '//' ~[\r\n]* -> skip
    ;