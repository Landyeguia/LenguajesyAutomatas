using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Generado
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {
        }
        public Lenguaje(string nombre) : base(nombre)
        {
        }
        public void Programa()
        {
            Librerias();
            Main();
        }
        public void Librerias()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            match(".");
            match("h");
            match(">");

            
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
                match("include");
                Main();
            }
            

            
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
                match("include");
                match("math");
                Main();
            }
            

            
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
                match("include");
            }
            
            match("#");
        }
        public void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            match("{");
            match("}");
        }
    }
}
