using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Helper enum for pass language.
    /// Differentiated using languages and countries.
    /// </summary>
    [Obsolete("This enum has been deprecated.")]
    public enum Language
    {

        AFRIKAANS_SOUTH_AFRICA,

        ALBANIAN_ALBANIA,

        ARABIC_ALGERIA,

        ARABIC_BAHRAIN,

        ARABIC_EGYPT,

        ARABIC_IRAQ,

        ARABIC_JORDAN,

        ARABIC_KUWAIT,

        ARABIC_LEBANON,

        ARABIC_LIBYA,

        ARABIC_MOROCCO,

        ARABIC_OMAN,

        ARABIC_QATAR,

        ARABIC_SAUDI_ARABIA,

        ARABIC_SYRIA,

        ARABIC_TUNISIA,

        ARABIC_UNITED_ARAB_EMIRATES,

        ARABIC_YEMEN,

        ARMENIAN_ARMENIA,

        AZERI_CYRILLIC_ARMENIA,

        AZERI_LATIN_AZERBAIJAN,

        BASQUE_BASQUE,

        BELARUSIAN_BELARUS,

        BULGARIAN_BULGARIA,

        CATALAN_CATALAN,

        CHINESE_CHINA,

        CHINESE_HONG_KONG_SAR,

        CHINESE_MACUS_SAR,

        CHINESE_SINGAPORE,

        CHINESE_TAIWAN,

        CHINESE_SIMPLIFIED,

        CHINESE_TRADITIONAL,

        CROATIAN_CROATIA,

        CZECH_CZECH_REPUBLIC,

        DANISH_DENMARK,

        DHIVEHI_MALDIVES,

        DUTCH_BELGIUM,

        DUTCH_NETHERLANDS,

        ENGLISH_AUSTRALIA,

        ENGLISH_BELIZE,

        ENGLISH_CANADA,

        ENGLISH_CARIBBEAN,

        ENGLISH_IRELAND,

        ENGLISH_JAMAICA,

        ENGLISH_NEW_ZEALAND,

        ENGLISH_PHILIPPINES,

        ENGLISH_SOUTH_AFRICA,

        ENGLISH_TRINIDAD_AND_TOBAGO,

        ENGLISH_UNITED_KINGDOM,

        ENGLISH_UNITED_STATES,

        ENGLISH_ZIMBABWE,

        ESTONIAN_ESTONIA,

        FAROESE_FAROE_ISLANDS,

        FARSI_IRAN,

        FINNISH_FINLAND,

        FRENCH_BELGIUM,

        FRENCH_CANADA,

        FRENCH_FRANCE,

        FRENCH_LUXEMBOURG,

        FRENCH_MONACO,

        FRENCH_SWITZERLAND,

        GALICIAN_GALICIAN,

        GEORGIAN_GEORGIA,

        GERMEN_AUSTRIA,

        GERMEN_GERMANY,

        GERMEN_LIENCHTENSTEIN,

        GERMEN_LUXEMBOURG,

        GERMEN_SWITZERLAND,

        GREEK_GREECE,

        GUJARATI_INDIA,

        HEBREW_ISRAEL,

        HINDI_INDIA,

        HUNGARIAN_HUNGARY,

        ICELANDIC_ICELAND,

        INDONESIAN_INDONESIA,

        ITALIAN_ITALY,

        ITALIAN_SWITZERLAND,

        JAPANESE_JAPAN,

        KANNADA_INDIA,

        KAZAKH_KAZAKHSTAN,

        KONKANI_INDIA,

        KOREAN_KOREA,

        KYRGYZ_KAZAKHSTAN,

        LATVIAN_LATVIA,

        LITHUANIAN_LITHUANIA,

        MACEDONIAN_FYROM,

        MALAY_BRUNEI,

        MALAY_MALAYSIA,

        MARATHI_INDIA,

        MONGOLIAN_MONGOLIA,

        NORWEGIAN_BOKMAL_NORWAY,

        NORWEGIAN_NYNORSK_NORWAY,

        POLISH_POLAND,

        PORTUGUESE_BRAZIL,

        PORTUGUESE_PORTUGAL,

        PUNJABI_INDIA,

        ROMANIAN_ROMANIA,

        RUSSIAN_RUSSIA,

        SANSKRIT_INDIA,

        SERBIAN_CYRILLIC_SERBIA,

        SERBIAN_LATIN_SERBIA,

        SLOVAK_SLOVAKIA,

        SLOVENIAN_SLOVENIAN,

        SPANISH_ARGENTINA,

        SPANISH_BOLIVIA,

        SPANISH_CHILE,

        SPANISH_COLOMBIA,

        SPANISH_COSTA_RICA,

        SPANISH_DOMINICAN_REPUBLIC,

        SPANISH_ECUADOR,

        SPANISH_ELSALVADOR,

        SPANISH_GUATEMALA,

        SPANISH_HONDURAS,

        SPANISH_MEXICO,

        SPANISH_NICARAGUA,

        SPANISH_PANAMA,

        SPANISH_PARAGUAY,

        SPANISH_PERU,

        SPANISH_PUERTO_RICO,

        SPANISH_SPAIN,

        SPANISH_URUGUAY,

        SPANISH_VENEZUELA,

        SWAHILI_KENYA,

        SWEDISH_FINLAND,

        SWEDISH_SWEDEN,

        SYRIAC_SYRIA,

        TAMIL_INDIA,

        TATAR_RUSSIA,

        TELUGU_INDIA,

        THAI_THAILAND,

        TURKISH_TURKEY,

        UKRAINIAN_UKRAINE,

        URDU_PAKISTAN,

        UZBEK_CYRILLIC_UZBEKISTAN,

        UZBEK_LATIN_UZEBEKISTAN,

        VIETNAMESE_VIETNAM
    }

    internal enum LanguageCode
    {
        af_za,

        sq_al,

        ar_dz,

        ar_bh,

        ar_eg,

        ar_iq,

        ar_jo,

        ar_kw,

        ar_lb,

        ar_ly,

        ar_ma,

        ar_om,

        ar_qa,

        ar_sa,

        ar_sy,

        ar_tn,

        ar_ae,

        ar_ye,

        hy_am,

        cy_az_az,

        lt_az_az,

        eu_es,

        be_by,

        bg_bg,

        ca_es,

        zh_cn,

        zh_hk,

        zh_mo,

        zh_sg,

        zh_tw,

        zh_chs,

        zh_cht,

        hr_hr,

        cs_cz,

        da_dk,

        div_mv,

        nl_be,

        nl_nl,

        en_au,

        en_bz,

        en_ca,

        en_cb,

        en_ie,

        en_jm,

        en_nz,

        en_ph,

        en_za,

        en_tt,

        en_gb,

        en_us,

        en_zw,

        et_ee,

        fo_fo,

        fa_ir,

        fi_fi,

        fr_be,

        fr_ca,

        fr_fr,

        fr_lu,

        fr_mc,

        fr_ch,

        gl_es,

        ka_ge,

        de_at,

        de_de,

        de_li,

        de_lu,

        de_ch,

        el_gr,

        gu_in,

        he_il,

        hi_in,

        hu_hu,

        is_is,

        id_id,

        it_it,

        it_ch,

        ja_jp,

        kn_in,

        kk_kz,

        kok_in,

        ko_kr,

        ky_kz,

        lv_lv,

        lt_lt,

        mk_mk,

        ms_bn,

        ms_my,

        mr_in,

        mn_mn,

        nb_no,

        nn_no,

        pl_pl,

        pt_br,

        pt_pt,

        pa_in,

        ro_ro,

        ru_ru,

        sa_in,

        cy_sr_sp,

        lt_sr_sp,

        sk_sk,

        sl_si,

        es_ar,

        es_bo,

        es_cl,

        es_co,

        es_cr,

        es_do,

        es_ec,

        es_sv,

        es_gt,

        es_hn,

        es_mx,

        es_ni,

        es_pa,

        es_py,

        es_pe,

        es_pr,

        es_es,

        es_uy,

        es_ve,

        sw_ke,

        sv_fi,

        sv_se,

        syr_sy,

        ta_in,

        tt_ru,

        te_in,

        th_th,

        tr_tr,

        uk_ua,

        ur_pk,

        cy_uz_uz,

        lt_uz_uz,

        vi_vn
    }
}
