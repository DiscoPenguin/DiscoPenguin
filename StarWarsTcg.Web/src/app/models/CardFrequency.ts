import { Card } from "./Card";

export class CardFrequency {
    cardId: number = 0;
    frequency: number = 0;
    totalQuantity: number = 0;
    card: Card | null = null;
    
    constructor(values: Object = {}) {
        Object.assign(this, values);
    }
}
