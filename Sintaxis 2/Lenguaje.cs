using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    Requerimiento 1: Mensajes del printf deben salir sin comillas
                     Incluir \n y \t como secuencias de escape
    Requerimiento 2: Agregar el % al PorFactor
                     Modifcar el valor de una variable con ++,--,+=,-=,*=,/=.%=
    Requerimiento 3: Cada vez que se haga un match(Tipos.Identificador) verficar el
                     uso de la variable
                     Icremento(), Printf(), Factor() y usar getValor y Modifica
                     Levantar una excepcion en scanf() cuando se capture un string
    Requerimiento 4: Implemenar la ejecución del ELSE
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main(true);
            Imprime();
        }

        private void Imprime()
        {
            log.WriteLine("-----------------");
            log.WriteLine("V a r i a b l e s");
            log.WriteLine("-----------------");
            foreach (Variable v in lista)
            {
                log.WriteLine(v.getNombre() + " " + v.getTiposDatos() + " = " + v.getValor());
            }
            log.WriteLine("-----------------");
        }

        private bool Existe(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return true;
                }
            }
            return false;
        }
        private void Modifica(string nombre, float nuevoValor)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        // Libreria -> #include<Identificador(.h)?>
        private void Libreria()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                match("h");
            }
            match(">");
        }
        //Librerias -> Libreria Librerias?
        private void Librerias()
        {
            Libreria();
            if (getContenido() == "#")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato ListaIdentificadores; Variables?
        private void Variables()
        {
            Variable.TiposDatos tipo = Variable.TiposDatos.Char;
            switch (getContenido())
            {
                case "int": tipo = Variable.TiposDatos.Int; break;
                case "float": tipo = Variable.TiposDatos.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TiposDatos tipo)
        {
            if (!Existe(getContenido()))
            {
                lista.Add(new Variable(getContenido(), tipo));
            }
            else
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> está duplicada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(tipo);
            }
        }
        //BloqueInstrucciones -> { ListaInstrucciones ? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "printf")
            {
                Printf(ejecuta);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            else
            {
                Asignacion(ejecuta);
            }
        }
        //Asignacion -> identificador = Expresion;
        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta)
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }

            string variable = getContenido();
            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");
                Expresion();
                float valor = stack.Pop();
                ModificarVariable(variable, valor);
            }
            else if (getContenido() == "++")
            {
                match("++");
                IncrementarVariable(variable, 1);
            }
            else if (getContenido() == "--")
            {
                match("--");
                IncrementarVariable(variable, -1);
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                float valor = stack.Pop();
                ModificarVariable(variable, getValor(variable) + valor);
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
                float valor = stack.Pop();
                ModificarVariable(variable, getValor(variable) - valor);
            }
            // Agrega casos para otros operadores de asignación aquí
            else
            {
                throw new Error("de sintaxis, se esperaba un operador de asignación válido", log, linea, columna);
            }

            match(";");
        }

        private void IncrementarVariable(string variable, float incremento)
        {
            float valor = getValor(variable);
            ModificarVariable(variable, valor + incremento);
        }

        private void ModificarVariable(string variable, float nuevoValor)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == variable)
                {
                    v.setValor(nuevoValor);
                    return;
                }
            }
        }
        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }

        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion(ejecuta);
            Condicion();
            match(";");
            Incremento(ejecuta);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
        }
        //Incremento -> Identificador ++ | --
        private void Incremento(bool ejecuta)
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);
            stack.Push(getValor(getContenido()));
            if (getContenido() == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float R1 = stack.Pop();
            float R2 = stack.Pop();

            switch (operador)
            {
                case "==": return R2 == R1;
                case ">": return R2 > R1;
                case ">=": return R2 >= R1;
                case "<": return R2 < R1;
                case "<=": return R2 <= R1;
                default: return R2 != R1;
            }
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        // ...

        //If -> if (Condicion) BloqueInstrucciones (else BloqueInstrucciones | Instruccion)?
private void If(bool ejecuta)
{
    match("if");
    match("(");
    bool evaluacion = Condicion();
    match(")");

    // Almacenar el estado actual de las variables antes de entrar al bloque if-else
    List<Variable> estadoAntes = new List<Variable>(lista);

    bool ejecutarBloqueIf = ejecuta && evaluacion; // Controla si se ejecuta el bloque "if"
    bool ejecutarBloqueElse = ejecuta && !evaluacion; // Controla si se ejecuta el bloque "else"

    // Verificar si hay un "else" anidado
    if (getContenido() == "{")
    {
        if (ejecutarBloqueIf)
        {
            BloqueInstrucciones(true);
        }
        else
        {
            BloqueInstrucciones(false);
        }
    }
    else
    {
        if (ejecutarBloqueIf)
        {
            Instruccion(true);
        }
        else
        {
            Instruccion(false);
        }
    }

    // Restaurar el estado de las variables después de salir del bloque if-else
    if (getContenido() == "else")
    {
        lista = estadoAntes; // Restaurar las variables al estado anterior
        match("else");

        if (getContenido() == "if")
        {
            // Si se encuentra un "if" anidado, llamamos recursivamente a la función "If"
            If(ejecutarBloqueElse && Condicion());
        }
        else if (getContenido() == "{")
        {
            if (ejecutarBloqueElse)
            {
                BloqueInstrucciones(true);
            }
            else
            {
                match("{");
                while (getContenido() != "}")
                {
                    Instruccion(false); // No ejecutar las instrucciones del primer "else"
                }
                match("}");
            }
        }
        else
        {
            if (ejecutarBloqueElse)
            {
                Instruccion(true);
            }
            else
            {
                Instruccion(false);
            }
        }
    }
}
private void Printf(bool ejecuta)
{
    match("printf");
    match("(");

    // Quitamos comillas
    // Pasamos la cadena a un String
    string contenido = getContenido();
    int pos = contenido.IndexOf('"');

    // Verificamos si se encontraron comillas
    if (pos >= 0)
    {
        contenido = contenido.Substring(pos + 1, contenido.Length - pos - 2);

        // Reemplazamos "\\n" con Environment.NewLine para los saltos de línea
        contenido = contenido.Replace("\\n", Environment.NewLine);

        // Reemplazamos "\\t" con "\t" para las tabulaciones
        contenido = contenido.Replace("\\t", "\t");

        if (ejecuta)
        {
            // Parseamos la cadena de formato para encontrar las variables a imprimir
            string[] partes = contenido.Split('%');
            for (int i = 0; i < partes.Length; i++)
            {
                if (i == 0)
                {
                    Console.Write(partes[i]);
                }
                else
                {
                    char formato = partes[i][0];
                    string variableNombre = partes[i].Substring(1); // Nombre de la variable

                    // Buscamos la variable por nombre y la imprimimos si existe
                    if (Existe(variableNombre))
                    {
                        float valor = getValor(variableNombre);
                        switch (formato)
                        {
                            case 'd':
                                Console.Write(valor); // Imprimir como número
                                break;
                            case 'f':
                                Console.Write(valor); // Imprimir como número de punto flotante
                                break;
                            case 's':
                                Console.Write(variableNombre); // Imprimir como cadena
                                break;
                            default:
                                Console.Write("%" + partes[i]); // Mantener el formato original si no se reconoce
                                break;
                        }
                    }
                    else
                    {
                        Console.Write("%" + partes[i]); // Mantener el formato original si la variable no existe
                    }
                }
            }
        }
    }
    match(Tipos.Cadena);

    if (getContenido() == ",")
    {
        Console.Write(" ");
        match(",");
        if (!Existe(getContenido()))
        {
            throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
        }
        else
        {
            if (ejecuta)
            {
                Console.Write(getValor(getContenido()));
            }
            match(Tipos.Identificador);
        }
    }

    match(")");
    match(";");
}
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
        {
            match("scanf");//Valida el Scanf
            match("(");
            match(Tipos.Cadena);//Busca la cadena ingresada
            match(",");
            match("&");//Termina la validacion del scanf
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }

            string variable = getContenido();// Guarda la variable
            match(Tipos.Identificador);
            if (ejecuta)
            {
                string captura = Console.ReadLine();//Guardamos captura
                if (captura != null)// Verifica que captura no este vacio
                {
                    try
                    {
                        float resultado = float.Parse(captura);
                        Modifica(variable, resultado);
                    }
                    catch (FormatException)
                    {
                        throw new Error("de conversión, la entrada no es un número decimal válido", log, linea, columna);
                    }
                }
                else
                {
                    throw new Error("de lectura, la entrada no puede ser nula", log, linea, columna);
                }
            }
            match(")");
            match(";");
        }

        //Main -> void main() BloqueInstrucciones
        private void Main(bool ejecuta)
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(ejecuta);
            // Mostrar los valores de las variables al final
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (operador == "+")
                    stack.Push(R1 + R2);
                else
                    stack.Push(R1 - R2);
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();

                if (operador == "*")
                {
                    stack.Push(R1 * R2);
                }
                else if (operador == "/")
                {
                    stack.Push(R1 / R2);
                }
                else if (operador == "%") // Agregar la operación de módulo (%)
                {
                    stack.Push(R1 % R2);
                }
            }
        }

        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }

                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}