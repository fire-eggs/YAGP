
namespace SharpGEDParser.Model
{
    public static class Tag
    {
        /// <summary>
        /// All "known" GEDCOM tags.
        /// 
        /// Maintain in alphabetic order (except INVALID) for ease of maintainence.
        /// Leading underscore should be ignored for alphabetization.
        /// </summary>
        public enum GedTag
        {
            INVALID,
            MISSING,

            ABBR,
            ADDR,
            ADOP,
            ADR1, // ADDR sub
            ADR2, // ADDR sub
            ADR3, // ADDR sub
            AFN,
            AGE,  // event sub; INDI attrib
            AGNC, // event sub; SOUR.DATA
            _AKA,
            ALIA,
            ANCI,
            ANUL,
            ASSO,
            AUTH,
            BAPL, // LDS
            BAPM,
            BARM,
            BASM,
            BIRT,
            BLES,
            BURI,
            CALN,
            CAST,
            CAUS, // event sub
            CENS,
            CHAN,
            CHAR,
            CHIL,
            CHRA,
            CHR,
            CITY, // ADDR sub
            CONC,
            CONF,
            CONL, // LDS
            CONT, // (multiple)
            COPR,
            CREM,
            CTRY, // ADDR sub
            DATA,
            DATE,
            DEAT,
            DESI,
            DEST,
            DIV,
            DIVF,
            DSCR,
            EDUC,
            EMAIL,
            EMIG,
            ENDL, // LDS
            ENGA,
            EVEN,
            FACT,
            FAM,
            FAMC,
            FAMS,
            FAX,
            FCOM,
            FILE,
            FORM,
            _FREL,
            GEDC,
            GIVN,
            GRAD,
            HEAD,
            HUSB,
            IDNO,
            IMMI,
            INDI,
            LANG,
            LVG,  // "Family Tree Maker"
            LVNG, // "generations"
            MARB,
            MARC,
            MARL,
            MARR,
            MARS,
            MEDI,
            _MREL,
            NAME,
            NATI,
            NATU,
            NCHI,
            NICK,
            NMR,
            NOTE,
            NPFX,
            NSFX,
            OBJE,
            OCCU,
            ORDN,
            PAGE,
            PEDI,
            PHON,
            PLAC,
            POST, // ADDR sub
            _PREF,
            PROB,
            PROP,
            PUBL,
            QUAY,
            REFN,
            RELA, // INDI.ASSO
            RELI,
            REPO,
            RESI,
            RESN,
            RETI,
            RFN,
            RIN,
            SEX,
            SLGC, // LDS
            SLGS, // LDS
            SOUR,
            SPFX,
            SSN,
            STAE, // ADDR sub
            STAT,
            _STAT,
            SUBM,
            SUBN,
            SURN,
            TEMP,
            TEXT,
            TIME,
            TITL,
            TRLR,
            TYPE, // event sub; OBJE sub
            UID,
            _UID,
            VERS,
            WIFE,
            WILL,
            WWW,
        }
    }
}
