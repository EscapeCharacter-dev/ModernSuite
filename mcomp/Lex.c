#include <mcomp/Front.h>
#include <string.h>
#include <ctype.h>
#include <stdlib.h>
#include <stdio.h>

#define strequal(x, y) !strcmp(##x, ##y)

static MLexTokKind resolveKeyword(const char *keyword_str)
{
	// im ashamed of myself because of this if-else ladder
	if (strequal(keyword_str, "if"))
		return MCOMP_TOK_IF;
	else if (strequal(keyword_str, "else"))
		return MCOMP_TOK_ELSE;
	else if (strequal(keyword_str, "switch"))
		return MCOMP_TOK_SWITCH;
	else if (strequal(keyword_str, "for"))
		return MCOMP_TOK_FOR;
	else if (strequal(keyword_str, "foreach"))
		return MCOMP_TOK_FOREACH;
	else if (strequal(keyword_str, "foreachm"))
		return MCOMP_TOK_FOREACHM;
	else if (strequal(keyword_str, "while"))
		return MCOMP_TOK_WHILE;
	else if (strequal(keyword_str, "do"))
		return MCOMP_TOK_DO;
	else if (strequal(keyword_str, "nooptz"))
		return MCOMP_TOK_NOOPTZ;
	else if (strequal(keyword_str, "function"))
		return MCOMP_TOK_FUNCTION;
	else if (strequal(keyword_str, "byte"))
		return MCOMP_TOK_BYTE;
	else if (strequal(keyword_str, "sbyte"))
		return MCOMP_TOK_SBYTE;
	else if (strequal(keyword_str, "short"))
		return MCOMP_TOK_SHORT;
	else if (strequal(keyword_str, "ushort"))
		return MCOMP_TOK_USHORT;
	else if (strequal(keyword_str, "least32"))
		return MCOMP_TOK_LEAST32;
	else if (strequal(keyword_str, "uleast32"))
		return MCOMP_TOK_ULEAST32;
	else if (strequal(keyword_str, "int"))
		return MCOMP_TOK_INT;
	else if (strequal(keyword_str, "uint"))
		return MCOMP_TOK_UINT;
	else if (strequal(keyword_str, "long"))
		return MCOMP_TOK_LONG;
	else if (strequal(keyword_str, "ulong"))
		return MCOMP_TOK_ULONG;
	else if (strequal(keyword_str, "octa"))
		return MCOMP_TOK_OCTA;
	else if (strequal(keyword_str, "uocta"))
		return MCOMP_TOK_UOCTA;
	else if (strequal(keyword_str, "half"))
		return MCOMP_TOK_HALF;
	else if (strequal(keyword_str, "single"))
		return MCOMP_TOK_SINGLE;
	else if (strequal(keyword_str, "double"))
		return MCOMP_TOK_DOUBLE;
	else if (strequal(keyword_str, "extended"))
		return MCOMP_TOK_EXTENDED;
	else if (strequal(keyword_str, "quad"))
		return MCOMP_TOK_QUAD;
	else if (strequal(keyword_str, "bool"))
		return MCOMP_TOK_BOOL;
	else if (strequal(keyword_str, "union"))
		return MCOMP_TOK_UNION;
	else if (strequal(keyword_str, "struct"))
		return MCOMP_TOK_STRUCT;
	else if (strequal(keyword_str, "pstruct"))
		return MCOMP_TOK_PSTRUCT;
	else if (strequal(keyword_str, "void"))
		return MCOMP_TOK_VOID;
	else if (strequal(keyword_str, "short_string"))
		return MCOMP_TOK_SHORTSTRING;
	else if (strequal(keyword_str, "string"))
		return MCOMP_TOK_STRING;
	else if (strequal(keyword_str, "long_string"))
		return MCOMP_TOK_LONGSTRING;
	else if (strequal(keyword_str, "cstring"))
		return MCOMP_TOK_CSTRING;
	else if (strequal(keyword_str, "using"))
		return MCOMP_TOK_USING;
	else if (strequal(keyword_str, "var"))
		return MCOMP_TOK_VAR;
	else if (strequal(keyword_str, "const"))
		return MCOMP_TOK_CONST;
	else if (strequal(keyword_str, "public"))
		return MCOMP_TOK_PUBLIC;
	else if (strequal(keyword_str, "clang"))
		return MCOMP_TOK_CLANG;
	else if (strequal(keyword_str, "return"))
		return MCOMP_TOK_RETURN;
	else if (strequal(keyword_str, "sizeof"))
		return MCOMP_TOK_SIZEOF;
	else if (strequal(keyword_str, "lengthof"))
		return MCOMP_TOK_LENGTHOF;
	else if (strequal(keyword_str, "goto"))
		return MCOMP_TOK_GOTO;
	else if (strequal(keyword_str, "label"))
		return MCOMP_TOK_LABEL;
	else
		return MCOMP_TOK_BAD;
}

static MFrontValue resolveIntegerDecimal(const char *text)
{
	const long length = strlen(text);

	for (long i = 0; i < length; i++)
	{
		if (isdigit(text[i]))
			continue;
		else
			return (MFrontValue) { 0 };
	}

	MFrontValue val;
	val.int64 = atoll(text);

	return val;
}

static int isHexCharacter(int c)
{
	if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || isdigit(c))
		return 1;
	return 0;
}

static FILE *stream;

void MLexOpen(const char *filepath)
{
#ifdef _MSC_VER
	if (fopen_s(&stream, filepath, "r"))
		goto _mlexoerr;
#else
	if (!(stream = fopen(filepath, "r")))
		goto _mlexoerr;
#endif
	return;
_mlexoerr:
	fprintf(stderr, "mcomp: Couldn't open file '%s'.\n", filepath);
	exit(0);
	return;
}

void MLexClose(void)
{
	fclose(stream);
	return;
}

#define fpeekc(x) fgetc(##x); fseek(##x, -1, SEEK_CUR)

MLexTok MLexNextToken(void)
{
	MLexTok ret = { 0 };
	int c = fgetc(stream);
	while (isspace(c))
		c = fgetc(stream);
	int nc = 0;

	switch (c)
	{
	case '+':
		nc = fpeekc(stream);
		if (nc == '+')
		{
			ret.Kind = MCOMP_TOK_PLUSPLUS;
			fseek(stream, 1, SEEK_CUR);
		}
		else if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_PLUSEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_PLUS;
		break;

	case '-':
		nc = fpeekc(stream);
		if (nc == '-')
		{
			ret.Kind = MCOMP_TOK_MINUSMINUS;
			fseek(stream, 1, SEEK_CUR);
		}
		else if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_MINUSEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_MINUS;
		break;

	case '*':
		nc = fpeekc(stream);
		if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_STAREQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_STAR;
		break;

	case '/':
		nc = fpeekc(stream);
		if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_SLASHEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_SLASH;
		break;

	case '%':
		nc = fpeekc(stream);
		if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_PERCENTEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_PERCENT;
		break;

	case '^':
		nc = fpeekc(stream);
		if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_HELMETEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_HELMET;
		break;

	case '|':
		nc = fpeekc(stream);
		if (nc == '|')
		{
			fseek(stream, 1, SEEK_CUR);
			nc = fpeekc(stream);
			if (nc == '=')
			{
				ret.Kind = MCOMP_TOK_PIPESEQUAL;
				fseek(stream, 1, SEEK_CUR);
			}
			else
				ret.Kind = MCOMP_TOK_PIPES;
		}
		else if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_PIPEEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_PIPE;
		break;

	case '&':
		nc = fpeekc(stream);
		if (nc == '&')
		{
			fseek(stream, 1, SEEK_CUR);
			nc = fpeekc(stream);
			if (nc == '=')
			{
				ret.Kind = MCOMP_TOK_AMPERSANDSEQUAL;
				fseek(stream, 1, SEEK_CUR);
			}
			else
				ret.Kind = MCOMP_TOK_AMPERSANDS;
		}
		else if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_AMPERSANDEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_AMPERSAND;
		break;

	case '!':
		nc = fpeekc(stream);
		if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_BANGEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_BANG;
		break;

	case '~':
		ret.Kind = MCOMP_TOK_TILDA;
		break;

	case '=':
		nc = fpeekc(stream);
		if (nc == '=')
		{
			ret.Kind = MCOMP_TOK_DEQUAL;
			fseek(stream, 1, SEEK_CUR);
		}
		else
			ret.Kind = MCOMP_TOK_EQUAL;
		break;

	case ':':
		ret.Kind = MCOMP_TOK_COLON;
		break;

	case ';':
		ret.Kind = MCOMP_TOK_SEMICOLON;
		break;

	case EOF:
		ret.Kind = MCOMP_TOK_EOF;
		break;

	default:
		break;
	}

	fseek(stream, 1, SEEK_CUR);
	return ret;
}