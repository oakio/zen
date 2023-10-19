# Zen
Experimental programming language.

Is a `LLVM` based compiler.

Inspired by: `C`, `GoLang`

## Examples
```c
bool is_prime(i32 v) {
    if (v <= 1) {
        return false;
    }
    i32 h = v / 2;
    i32 x = 2;
    while (x <= h) {
        if (v % x == 0) {
            return false;
        }
        x = x + 1;
    }
    return true;
}

i32 main(i32 n) {
    i32 x = 1;
    i32 count = 0;
    while (x <= n) {
        if (is_prime(x)) {
            count = count + 1;
        }
        x = x + 1;
    }
    return count;
}
```