public abstract record GameResult(bool IsClear);

public record ClearResult(

    int Score,
    int KillCount
    // ‚±‚±‚ÉClearResult‚Ìƒf[ƒ^‚ğ’Ç‰Á

    ) : GameResult(IsClear: true);

public abstract record FailedResult() : GameResult(IsClear: false);

public record DeadFailedResult(
    
    ) : FailedResult();

public record TimeOverFailedResult(
    
    ) : FailedResult();

