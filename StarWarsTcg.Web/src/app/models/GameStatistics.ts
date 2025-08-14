
export class GameStatistics {

    public userId: string;
    public avatarUrl: string; //TODO: Fix this up to match the User.AvatarId + lookup
    public firstName: string;
    public lastName: string;

    public totalGames: number;
    public lightGames: number;
    public lightWins: number;
    public darkGames: number;
    public darkWins: number;
    public totalWins: number;

    /**
     * Constructor for the GameStatistics class. (vwGameStatistics)
     * Initializes all properties with default values or provided ones.
     * @param initialData An optional object to initialize the properties.
     */
    constructor(initialData?: Partial<GameStatistics>) {
        this.userId = initialData?.userId ?? '';
        this.avatarUrl = initialData?.avatarUrl ?? '';
        this.firstName = initialData?.firstName ?? '';
        this.lastName = initialData?.lastName ?? '';
        
        this.totalGames = initialData?.totalGames ?? 0;
        this.lightGames = initialData?.lightGames ?? 0;
        this.lightWins = initialData?.lightWins ?? 0;
        this.darkGames = initialData?.darkGames ?? 0;
        this.darkWins = initialData?.darkWins ?? 0;
        this.totalWins = initialData?.totalWins ?? 0;
    }

}
