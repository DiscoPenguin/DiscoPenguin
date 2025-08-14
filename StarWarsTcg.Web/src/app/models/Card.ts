export class Card {
    id: number = 0;
    name: string = '';
    expansionSet: string = '';
    imageFile: string = '';
    side: string = '';
    type: string = '';
    subtype: string = '';
    cost: string = '';
    speed: string = '';
    power: string = '';
    health: string = '';
    rarity: string = '';
    number?: number = 0;
    usage?: string = '';
    text?: string = '';
    script?: string = '';
    classification?: string = '';

    constructor(
        id: number,
        name: string,
        expansionSet: string,
        imageFile: string,
        side: string,
        type: string,
        subtype: string,
        cost: string,
        speed: string,
        power: string,
        health: string,
        rarity: string,
        number?: number,
        usage?: string,
        text?: string,
        script?: string,
        classification?: string
    ) {
        this.id = id;
        this.name = name;
        this.expansionSet = expansionSet;
        this.imageFile = imageFile;
        this.side = side;
        this.type = type;
        this.subtype = subtype;
        this.cost = cost;
        this.speed = speed;
        this.power = power;
        this.health = health;
        this.rarity = rarity;
        this.number = number;
        this.usage = usage;
        this.text = text;
        this.script = script;
        this.classification = classification;
    }

}
