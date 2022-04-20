using System;
using System.Collections.Generic;
using System.Text;

namespace eVidaGeneralLib.Util {
	public class DateUtil {

        private static DateTime[] RECURRENT_HOLIDAY = new DateTime[] { 			
		    new DateTime(2000, 1, 1),
		    new DateTime(2000, 4, 21),
		    new DateTime(2000, 5, 1),
		    new DateTime(2000, 9, 7), 
		    new DateTime(2000, 10, 12), 
		    new DateTime(2000, 11, 2), 
		    new DateTime(2000, 11, 15), 
		    new DateTime(2000, 11, 30), 
		    new DateTime(2000, 12, 25)
	    };

        private static DateTime[] FIXED_HOLIDAY = new DateTime[] {
		    new DateTime(2017, 02, 27), 
		    new DateTime(2017, 02, 28),
		    new DateTime(2017, 03, 01), 
		    new DateTime(2017, 04, 14),
		    new DateTime(2017, 06, 15),
		    new DateTime(2017, 10, 13),
		    new DateTime(2018, 02, 12),
		    new DateTime(2018, 02, 13),
		    new DateTime(2018, 02, 14),
		    new DateTime(2018, 03, 30),
		    new DateTime(2018, 05, 31)
	    };

		public static int CalculaIdade(DateTime dt) {
			return YearDiff(DateTime.Now.Date, dt);
		}
		public static int YearDiff(DateTime dt, DateTime dt2) {
			int diff = dt.Year - dt2.Year;

			// Se dt ainda não chegou no mesmo dia do ano, subtrai
			if (dt.DayOfYear < dt2.DayOfYear)
				diff--;

			return diff;
		}

		public static int DayDiff(DateTime dtMaior, DateTime dtMenor, int minDiff) {
			TimeSpan diff = dtMaior.Subtract(dtMenor);

			int d = diff.Days;
			if (d < minDiff) d = minDiff;
			return d;
		}

		public static DateTime PreviousBusinessDay(DateTime dt) {
			return NextBusinessDay(dt, -1);
		}
		public static DateTime NextBusinessDay(DateTime dt) {
			return NextBusinessDay(dt, 1);
		}

		private static DateTime NextBusinessDay(DateTime dt, int shift) {
			dt = dt.AddDays(shift).Date;
			while (!IsBusinessDay(dt))
				dt = dt.AddDays(shift);
			return dt;
		}

		public static bool IsBusinessDay(DateTime dt) {
			bool weekday = !(dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday);
			if (weekday) {
				return !IsHoliday(dt);
			} else {
				return false;
			}
		}
		private static bool IsHoliday(DateTime dt) {
			return IsFixedHoliday(dt) || IsRecurrentHoliday(dt);
		}

		private static bool IsFixedHoliday(DateTime dt) {
			return Array.BinarySearch(FIXED_HOLIDAY, dt) >= 0;
		}

		private static bool IsRecurrentHoliday(DateTime dt) {
			foreach (DateTime holiday in RECURRENT_HOLIDAY) {
				if (holiday.Day == dt.Day && holiday.Month == dt.Month)
					return true;
			}
			return false;
		}

		public static double CalcularHorasDiasUteis(DateTime inicio, DateTime fim) {
			if (inicio.Date.Equals(fim.Date)) {
				if (!IsBusinessDay(inicio))
					return 0;
				return Math.Round(fim.Subtract(inicio).TotalHours,3);
			} else {
				DateTime nextBusiness = DateUtil.NextBusinessDay(inicio);
				DateTime nextDay = inicio.AddDays(1).Date;
				DateTime ultDia = fim.Date;
				double horas = 0;
				if (IsBusinessDay(inicio))
					horas += nextDay.Subtract(inicio).TotalHours;
				while (nextBusiness < ultDia) {
					horas += 24;
					nextBusiness = DateUtil.NextBusinessDay(nextBusiness);
				}
				if (IsBusinessDay(ultDia))
					horas += fim.Subtract(ultDia).TotalHours;
				return Math.Round(horas,3);
			}
		}
		/// <summary>
		/// Checa se dois períodos são concorrentes, ou seja, possuem dias em comum
		/// </summary>
		/// <param name="dtInicio1"></param>
		/// <param name="dtFim1"></param>
		/// <param name="dtInicio2"></param>
		/// <param name="dtFim2"></param>
		/// <returns>Retorna true se são concorrentes</returns>
		public static bool CheckConcorrencia(DateTime dtInicio1, DateTime dtFim1, DateTime dtInicio2, DateTime dtFim2) {

			// Verifica se dtInicio2 está entre inicio e fim1
			if (dtInicio2.CompareTo(dtInicio1) >= 0 &&
				dtInicio2.CompareTo(dtFim1) <= 0)
				return true;

			// Verifica se dtFim2 está entre inicio e fim1
			if (dtFim2.CompareTo(dtInicio1) >= 0 &&
				dtFim2.CompareTo(dtFim1) <= 0)
				return true;

			// Verifica se o periodo 2 engloba todo o periodo 1
			if (dtInicio2.CompareTo(dtInicio1) <= 0 &&
				dtFim2.CompareTo(dtFim1) >= 0)
				return true;
			return false;
		}

        public static string FormatHours(double hours)
        {
            DateTime dtRef = DateTime.Now.Date;
            DateTime dtCalc = dtRef.AddHours(hours);
            TimeSpan ts = dtCalc.Subtract(dtRef);
            string str = "";
            if (ts.Days != 0)
                str += ts.Days + " dia(s) ";
            if (ts.Hours != 0)
                str += ts.Hours + " hora(s) ";
            if (ts.Minutes != 0)
                str += ts.Minutes + " minuto(s) ";
            if (ts.Seconds != 0)
                str += ts.Seconds + " segundo(s) ";
            if (string.IsNullOrEmpty(str))
                str = "-";
            return str;
        }	

        public static string FormatDateYMDToDMY(string data) {
            return data.Trim().Substring(6, 2) + "/" + data.Trim().Substring(4, 2) + "/" + data.Trim().Substring(0, 4);
        }

        // Soma uma data a um número de dias úteis
        public static DateTime SomarDiasUteis(DateTime dataInicial, int numDias)
        {
            if (numDias <= 0)
                return dataInicial;

            int i = 0;
            DateTime dataFinal = dataInicial;

            do
            {
                dataFinal = dataFinal.AddDays(1);

                // Se for dia útil
                if (IsBusinessDay(dataFinal))
                {
                    i++;
                }

            } while (i < numDias);

            return dataFinal;
        }

        // Subtrai um número de dias úteis de uma data
        public static DateTime SubtrairDiasUteis(DateTime dataInicial, int numDias)
        {
            if (numDias <= 0)
                return dataInicial;

            int i = 0;
            DateTime dataFinal = dataInicial;

            do
            {
                dataFinal = dataFinal.AddDays(-1);

                // Se for dia útil
                if (IsBusinessDay(dataFinal))
                {
                    i++;
                }

            } while (i < numDias);

            return dataFinal;
        }	
	}
}
