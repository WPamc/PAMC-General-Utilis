using System.Collections.Generic;
using ExcelMapper.Models;

namespace ExcelMapper.Data
{
    public static class HardcodedSourceRegistry
    {
        public static List<SqlSource> GetSources()
        {
            return new List<SqlSource>
            {
                new SqlSource("Claims", new List<string>
                {
                    "HPCODE", "MEMBID", "SUBSSN", "CLAIMNO", "TBLROWID", "OPT", "DATERECD", "DATEPAID", 
                    "STATUS", "CLAIMTYPE", "INPATDAYS", "FROMDATESVC", "TODATESVC", "BILLED", "NET", 
                    "DIAGCODE", "DIAGDESC", "MAIN_FIRSTNM", "MAIN_LASTNM", "MAIN_PATID", "DEP_FIRSTNM", 
                    "DEP_LASTNM", "DEP_SEX", "DEP_RLSHIP", "RELATIONSHIP_DESCR", "VENDORNM", 
                    "BENEFIT_DESCR", "EXTERNAL_ADJ_DESCR", "PROVIDER_LASTNAME", "PROVIDER_FIRSTNAME", 
                    "REASON_ADJCODE", "REASON_COMMENTS", "REASON_DESCR", "UNDERWRITER_CLAIM_STATUS"
                }),
                new SqlSource("Members", new List<string>
                {
                    "HPCODE", "MEMBID", "SUBSSN", "FIRSTNM", "LASTNM", "SEX", "BIRTH", "RLSHIP", 
                    "DESCR", "HPNAME", "OPT", "EMPGROUP", "OPFROMDT", "OPTHRUDT", "HPFROMDT", 
                    "HPTHRUDT", "BILLRELATION1", "BILLRELATION2", "BILLRELATION3", "BILLRELATION4", 
                    "BILLRELATION5", "BILLRELATION6", "BILLRELATION7", "BILLRELATION8", 
                    "BILLRELATION9", "BILLRELATION10"
                })
            };
        }
    }
}
