/**
 * Represents an empty or null GUID string.
 * This is the standard "all zeros" GUID.
 */
export const EMPTY_GUID:string = '00000000-0000-0000-0000-000000000000';

/**
 * Optional: If you need a function to check if a GUID is empty.
 */
export function isEmptyGuid(guid: string | null | undefined): boolean {
  if (guid === null || typeof guid === 'undefined') {
    return true; // Or handle as per your application's null/undefined GUID policy
  }
  return guid === EMPTY_GUID;
}