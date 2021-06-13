#include <stdio.h>
#include <mcomp/Front.h>

int main()
{
	MLexOpen("Test.mn");
	MLexTok tok;
	do
	{
		tok = MLexNextToken();
		printf("%d\n", tok.Kind);
	} while (tok.Kind != MCOMP_TOK_EOF);
	MLexClose();
	return 0;
}