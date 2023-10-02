#include <stdio.h>
#include <math.h>
#include <iostream>

char a;
int b, i;
float c;

void main() // Funcion principal
{
    c = 20;
    printf("C = ", c);
    a = (float)((char)(c) + (float)(b));
    if (2 == 2)
    {
        printf("\nHola");
        if (4 == 1)
        {
            printf(" a todos");
        }
        else
        {
            printf(" a nadie");

            for (i=0;i<10;i++)
            {
                printf("\nHola",i);
                i++;
            }
        }
    }
    else
    {
        printf("mundo\n");
    }
}
