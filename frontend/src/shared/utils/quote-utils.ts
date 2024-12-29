import { authService } from '../services/auth.service';

export async function copyToClipboard(text: string) {
  await window.navigator.clipboard.writeText(text);
}

export function openDetailsPage(id: string) {
  // const jwt = authService.getToken();
  const url = import.meta.env.VITE_ADMINUI_BASE_URL + `api/quotedetails/?quoteid=${id}`;// &jwt=${jwt}`;
  window.open(url, '_blank')?.focus();
}
