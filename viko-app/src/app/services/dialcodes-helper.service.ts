import { COUNTRY_CODES } from '../interfaces/country-codes'; // [{ dial: '+351', emoji:'🇵🇹' }, ...]

const EXIT_PREFIXES = /^(00|011|0011|810)+/; 
// Pré-processa: cria lista de prefixos só com dígitos e ordena por comprimento desc.
const DIAL_DIGITS = [...COUNTRY_CODES]
    .map(dc => ({ dial: dc.code, digits: dc.code.replace(/\D/g, '') })) // "+351" -> "351"
    .sort((a, b) => b.digits.length - a.digits.length); // longest-first

/**
 * Separa um telefone internacional no formato "351912345678" (ou "+351912345678", "00351912345678", etc.)
 * em { indicative: "+351", number: "912345678" }.
 */
export function splitDialAndNumber(input: string, fallbackDial: string = '') {
  if (!input) return { countryCode: fallbackDial, number: '' };

  // Normaliza: mantém só dígitos, remove prefixos internacionais (00/011/0011/810…)
  let digitsOnly = String(input).replace(/\D/g, '');
  digitsOnly = digitsOnly.replace(EXIT_PREFIXES, '');

  // Tenta casar com o prefixo mais longo possível
  for (const p of DIAL_DIGITS) {
    if (digitsOnly.startsWith(p.digits)) {
      return {
        countryCode: p.dial,                          // ex.: "+351"
        number: digitsOnly.slice(p.digits.length)    // ex.: "912345678"
      };
    }
  }

  // Se nada casar, usa fallback e considera tudo como número
  return { countryCode: fallbackDial, number: digitsOnly };
}