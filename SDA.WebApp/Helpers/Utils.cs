using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Helpers
{
    public class Utils
    {


        public List<Tuple<int, string>> Months = new List<Tuple<int, string>>();

        public Utils()
        {
            Months.Add(Tuple.Create(1, "Enero"));
            Months.Add(Tuple.Create(2, "Febrero"));
            Months.Add(Tuple.Create(3, "Marzo"));
            Months.Add(Tuple.Create(4, "Abril"));
            Months.Add(Tuple.Create(5, "Mayo"));
            Months.Add(Tuple.Create(6, "Junio"));
            Months.Add(Tuple.Create(7, "Julio"));
            Months.Add(Tuple.Create(8, "Agosto"));
            Months.Add(Tuple.Create(9, "Septiembre"));
            Months.Add(Tuple.Create(10, "Octubre"));
            Months.Add(Tuple.Create(11, "Noviembre"));
            Months.Add(Tuple.Create(12, "Diciembre"));
        }

        

        public static string GetCorrelativo(int year, int tipoNomenclaturaId,
            ApplicationDbContext ctx)
        {

            CorrelativoAnual correlativo = (from u in ctx.CorrelativosAnual
                                            where u.anio == year
                                            select u).Single();
            TipoNomenclatura nomenclatura = (from u in ctx.TiposNomenclatura
                                             where u.tipoNomenclaturaId == tipoNomenclaturaId
                                             select u
                                            ).Single();
            int corr = correlativo.correlativo;
            correlativo.correlativo = corr + 1;

            ctx.SaveChanges();

            return nomenclatura.codigo + year.ToString().Substring(2) + corr.ToString("000");

        }

        public static string GetParameter(string key, ApplicationDbContext ctx)
        {

            var par = ctx.Parametros
                .SingleOrDefault(p => p.Codigo == key);

            if (par == null)
                return "";

            return par.Valor;
        }

        public static string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
        // Validacion de NIT
        public static bool ValidarNIT(string i_nit)
        {
            int i = 0;
            int vSuma = 0;
            int vValor = 0;
            int digito = 0;
            int w_inttemp = 0;
            int vDiv = 0;
            int vMul = 0;
            int vResta = 0;
            bool nit_correcto = false;

            if (string.IsNullOrEmpty(i_nit) | i_nit.Length < 14)
            {
                nit_correcto = false;
            }
            else
            {
                vValor = 14;
                w_inttemp = int.Parse(i_nit.Substring(10, 3));

                if ((w_inttemp < 100))
                {
                    for (i = 0; i <= 12; i++)
                    {
                        w_inttemp = int.Parse(i_nit.Substring(i, 1));
                        vSuma = vSuma + (w_inttemp * vValor);
                        vValor = vValor - 1;
                    }

                    vDiv = vSuma / 11;
                    vMul = vDiv * 11;
                    vResta = vSuma - vMul;
                    digito = int.Parse(i_nit.Substring(13, 1));
                    if ((vResta == 10))
                    {
                        vResta = 0;
                    }

                    if (vResta == digito)
                    {
                        nit_correcto = true;
                    }
                    else
                    {
                        nit_correcto = false;
                    }
                }
                else
                {
                    w_inttemp = int.Parse(i_nit.Substring(10, 3));
                    if (w_inttemp > 100)
                    {
                        vValor = 2;
                        for (i = 0; i <= 12; i++)
                        {
                            w_inttemp = int.Parse(i_nit.Substring(i, 1));
                            vSuma = vSuma + w_inttemp * vValor;
                            if (vValor == 2)
                            {
                                vValor = 7;
                            }
                            else
                            {
                                vValor = vValor - 1;
                            }
                        }
                        vDiv = (vSuma / 11);
                        vMul = vDiv * 11;
                        vResta = vSuma - vMul;

                        if (vResta > 1)
                        {
                            vResta = 11 - vResta;
                            w_inttemp = int.Parse(i_nit.Substring(13, 1));
                            if (vResta == w_inttemp)
                            {
                                nit_correcto = true;
                            }
                            else
                            {
                                w_inttemp = int.Parse(i_nit.Substring(13, 1));
                                if (((vResta == 11) & (w_inttemp == 0)))
                                {
                                    nit_correcto = true;
                                }
                                else
                                {
                                    nit_correcto = false;
                                }
                            }
                        }
                        else
                        {
                            vResta = 0;
                            w_inttemp = int.Parse(i_nit.Substring(13, 1));
                            if ((vResta == w_inttemp))
                            {
                                nit_correcto = true;
                            }
                            else
                            {
                                nit_correcto = false;
                            }
                        }
                    }
                    else
                    {
                        nit_correcto = false;
                    }
                }
            }

            return nit_correcto;
        }
    }
}