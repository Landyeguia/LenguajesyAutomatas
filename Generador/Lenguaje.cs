using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Generador
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje() { }

        public Lenguaje(string nombre)
         : base(nombre) { }

        public void generaLenguaje()
        {
            generado.WriteLine("using System;");
            generado.WriteLine("using System.Collections.Generic;");
            generado.WriteLine("using System.Linq;");
            generado.WriteLine("using System.Reflection.PortableExecutable;");
            generado.WriteLine("using System.Threading.Tasks;");
            generado.WriteLine();
            generado.WriteLine("namespace Generado");
            generado.WriteLine("{");
            generado.WriteLine("    public class Lenguaje : Sintaxis");
            generado.WriteLine("    {");
            generado.WriteLine("        public Lenguaje()");
            generado.WriteLine("        {");
            generado.WriteLine("        }");
            generado.WriteLine("        public Lenguaje(string nombre) : base(nombre)");
            generado.WriteLine("        {");
            generado.WriteLine("        }");

            Producciones();

            generado.WriteLine("    }");
            generado.WriteLine("}");
        }
     
        private void Producciones()
        {
            generado.WriteLine("        public void " + getContenido() + ("()"));
            generado.WriteLine("        {");
            match(Tipos.SNT);
            match(Tipos.Flechita);
            listaSimbolos();
            match(Tipos.FinProduccion);
            generado.WriteLine("        }");

            if (getClasificacion() == Tipos.SNT)
            {
                Producciones();
            }
        }
        private void listaSimbolos()
        {
            if (esPalabraReservada(getContenido()))
            {
                generado.WriteLine("            match(Tipos." + getContenido() + ");");
                match(Tipos.SNT);
            }
            else if (getClasificacion() == Tipos.ST)
            {
                generado.WriteLine("            match(\"" + getContenido() + "\");");
                match(Tipos.ST);
            }
            else if (getClasificacion() == Tipos.SNT)
            {
                generado.WriteLine("            " + getContenido() + "();");
                match(Tipos.SNT);
            }
            else if (getClasificacion() == Tipos.Epsilon)
            {
                match(Tipos.Epsilon);
                match(Tipos.PIzq);
                Epsilon();
                Ors();
                match(Tipos.PDer);
            }
            else if (getClasificacion() == Tipos.Or)
            {
                match(Tipos.Or);
                match(Tipos.PIzq);
                Epsilon();
                Ors();
                match(Tipos.PDer);
            }
            if (getClasificacion() != Tipos.FinProduccion)
            {
                listaSimbolos();
            }
        }
        private void Epsilon()
        {
            string simbolo = getContenido();
            generado.WriteLine();
            generado.WriteLine("            ");

            if (esPalabraReservada(simbolo))

            else if (getClasificacion() == Tipos.ST)
            {
                generado.WriteLine("            if (getContenido() == \"" + simbolo + "\")");
                match(Tipos.ST);
                generado.WriteLine("            {");
                generado.WriteLine("                match(\"" + simbolo + "\");");
            }

            while (true)
            {
                if (esPalabraReservada(getContenido()))
                {
                    generado.WriteLine("                match(Tipos." + getContenido() + ");");
                    match(Tipos.SNT);
                }
                else if (getClasificacion() == Tipos.ST)
                {
                    generado.WriteLine("                match(\"" + getContenido() + "\");");
                    match(Tipos.ST);
                }
                else if (getClasificacion() == Tipos.SNT)
                {
                    generado.WriteLine("                " + getContenido() + "();");
                    match(Tipos.SNT);
                }
                else
                {
                    break;
                }
            }

            generado.WriteLine("            }");
        }

        private void Ors()
        {
            string simbolo = getContenido();
            generado.WriteLine("            ");

            if (esPalabraReservada(simbolo))
            {
                match(Tipos.SNT);
                if (getClasificacion() == Tipos.PDer)
                {
                    generado.WriteLine("        else");
                }
                else
                {
                    generado.WriteLine("            if (getClasificacion() == Tipos." + simbolo + ")");
                }
                generado.WriteLine("            {");
                generado.WriteLine("                match(Tipos." + simbolo + ");");
                generado.WriteLine("            }");
            }
            else if (getClasificacion() == Tipos.ST)
            {
                match(Tipos.ST);
                if (getClasificacion() != Tipos.PDer)
                {
                    generado.WriteLine("            else if (getContenido() == \"" + simbolo + "\")");
                }
                else
                {
                    generado.WriteLine("            else");
                }
                generado.WriteLine("            {");
                generado.WriteLine("                match(\"" + simbolo + "\");");
                generado.WriteLine("            }");
            }
            else if (getClasificacion() == Tipos.SNT)
            {
                generado.WriteLine("            else");
                match(Tipos.SNT);
                generado.WriteLine("            {");
                generado.WriteLine("                " + simbolo + "();");
                generado.WriteLine("            }");
            }

            if (getClasificacion() != Tipos.PDer)
            {
                Ors();
            }
        }
        private bool esPalabraReservada(string palabra)
        {
            switch (palabra)
            {
                case "Identificador":
                case "Numero":
                case "Asignacion":
                case "Inicializacion":
                case "OperadorRelacional":
                case "OperadorTermino":
                case "OperadorFactor":
                case "IncrementoTermino":
                case "IncrementoFactor":
                case "Cadena":
                case "Ternario":
                case "FinSentencia":
                case "OperadorLogico":
                case "Inicio":
                case "Fin":
                case "Caracter":
                case "TipoDato":
                case "Zona":
                case "Condicion":
                case "Ciclo":
                    return true;
            }
            return false;
        }
    }
}