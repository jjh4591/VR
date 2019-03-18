#include stdio.h
#include stdlib.h

int main(){
    char ch;
    printf("Hello, world!");
    printf("문자입력 : ")
    scanf("%c", &ch);

    
    printf("입력된 문자 : \"%c\"입니다", ch);
    return 0;
}