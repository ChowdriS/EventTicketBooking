export function unwrapValues<T>(obj: any): T[] {
  return obj?.$values ?? [];
}
