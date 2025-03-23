using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph_Plotting : MonoBehaviour
{
    public string function;
    public float LimX = 9;
    public float LimY = 5;
    public GameObject point;
    private List<GameObject> Graph = new List<GameObject>();

    private Dictionary<char, int> prec = new Dictionary<char, int>();

    float FunctionsPool( string fc, float x ){
        if( fc == "sin" ) return Mathf.Sin(x);
        if( fc == "cos" ) return Mathf.Cos(x);
        if( fc == "tan" ) return Mathf.Tan(x);
        
        if( fc == "arcsin" ) return Mathf.Asin(x);
        if( fc == "arccos" ) return Mathf.Acos(x);
        if( fc == "arctan" ) return Mathf.Atan(x);

        if( fc == "ln" ) return Mathf.Log(x, 2.718281828f);
        if( fc == "log" ) return Mathf.Log(x, 2);

        if( fc == "sqrt" ) return Mathf.Sqrt(x);
        if( fc == "qbrt" ) return Mathf.Pow(x, 0.33333333f);

        if( fc == "abs" ) return Mathf.Abs(x);
        
        return -13;
    }

    // Start is called before the first frame update
    void Start()
    {
        prec['('] = 0;
        prec[')'] = 0;
        prec['+'] = 1;
        prec['-'] = 1;
        prec['*'] = 2;
        prec['/'] = 2;
        prec['^'] = 3;

        //int y = Solve(function, X);
        //Debug.Log(y);
    }

    int strlen( char[] s ){
        int n = 0;
        for( int i=0; s[i] != 0; i++ )
            n++;
        return n;
    }

    bool isnumber( char s ){
        return ('0' <= s && s <= '9' || s == '.');
    }

    bool isalpha( char s ){
        return (('a' <= s && s <= 'z') || ('A' <= s && s <= 'Z'));
    }

    private float rez( float val1, float val2, char op )
    {
        if( op == '+' )
            return val1 + val2;
        if( op == '-' )
            return val1 - val2;
        if( op == '*' )
            return val1 * val2;
        if( op == '/' )
            return val1 / val2;
        if( op == '^' )
            return Mathf.Pow(val1, val2);
        return 0;
    }

    string Prepare( string s ){
        string aux = "";
        int n = s.Length;
        for( int i = 0; i < n; i++ ){
            if( s[i] == ' ' ) continue;
            aux = aux + s[i];
            if( i < n - 1 && isnumber(s[i]) && (s[i + 1] == '(' || isalpha(s[i + 1])) ) aux = aux + '*';
        }
        return aux;
    }

    float Solve( string s, float x )
    {
        Stack<char> ops = new Stack<char>();
        Stack<float> values = new Stack<float>();

        s = Prepare(s);

        int i;
        int n = s.Length;
        ops.Clear();
        values.Clear();

        for( i=0; i<n; i++ ){
            if( s[i] == 'p' && i < n-1 && s[i] == 'i' ){
                values.Push(3.141592653f);
                continue;
            }
            if( s[i] == 'e' ){
                values.Push(2.718281828f);
                continue;
            }
            if( s[i] == 'x' ){
                values.Push(x);
                continue;
            }
            if( isnumber(s[i]) ){
                string nr = "";
                while( i < n && isnumber(s[i]) ){
                    nr = nr + s[i];
                    i++;
                }
                i--;
                float value = float.Parse(nr);
                values.Push(value);
                continue;
            }
            if( s[i] == '(' ){
                ops.Push(s[i]);
                continue;
            }
            if( s[i] == ')' ){
                while( ops.Count > 0 && ops.Peek() != '(' ){
                    float val1 = 1, val2 = 1;
                    if(values.Count > 0){
                        val2 = values.Peek();
                        values.Pop();
                    }
                    if(values.Count > 0){
                        val1 = values.Peek();
                        values.Pop();
                    }

                    char op = '\0';
                    op = ops.Peek();
                    ops.Pop();

                    values.Push( rez(val1, val2, op) );
                }
                ops.Pop();
                continue;
            }
            if( s[i] == '+' || s[i] == '-' || s[i] == '*' || s[i] == '/' || s[i] == '^' ){
                while( ops.Count > 0 && prec[ ops.Peek() ] >= prec[ s[i] ] ){
                    float val1 = 1, val2 = 1;
                    if(values.Count > 0){
                        val2 = values.Peek();
                        values.Pop();
                    }
                    if(values.Count > 0){
                        val1 = values.Peek();
                        values.Pop();
                    }

                    char op = '\0';
                    op = ops.Peek();
                    ops.Pop();

                    values.Push( rez(val1, val2, op) );
                }
                ops.Push(s[i]);
                continue;
            }
            if( isalpha(s[i]) ){
                string fc = "", arg = "";
                while( i < n && isalpha(s[i]) ){
                    fc = fc + s[i];
                    i++;
                }
                int paranteze = 0;
                do{
                    arg = arg + s[i];
                    if( s[i] == '(' ) paranteze++;
                    if( s[i] == ')' ) paranteze--;
                    i++;
                }while(paranteze != 0);
                i--;
                values.Push( FunctionsPool(fc, Solve(arg, x)) );
                continue;
            }
            if( s[i] == '|' ){
                string arg = "";
                do{
                    if( s[i] != '|' ) arg = arg + s[i];
                    i++;
                }while( s[i] != '|' );
                i--;
                values.Push( FunctionsPool("abs", Solve(arg, x)) );
            }
        }

        while( ops.Count > 0 ){
            float val1 = 1, val2 = 1;
            if(values.Count > 0){
                val2 = values.Peek();
                values.Pop();
            }
            if(values.Count > 0){
                val1 = values.Peek();
                values.Pop();
            }

            char op = '\0';
            op = ops.Peek();
            ops.Pop();

            values.Push( rez(val1, val2, op) );
        }

        if(values.Count > 0) return values.Peek();
        else return -13;
    }

    void DrawGraph(string function){
        if( Graph.Count > 0 ){
            foreach( GameObject point in Graph )
                Destroy(point);
        }

        float x = 0, y = 0;
        for( x=-LimX; x <=LimX; x += 0.005f ){
            y = Solve(function, x);
            if( y <= LimY && y >= -LimY ) Graph.Add(Instantiate(point, new Vector3(x, y, 0), Quaternion.identity, gameObject.transform));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Return) ) DrawGraph(function);
    }
}
