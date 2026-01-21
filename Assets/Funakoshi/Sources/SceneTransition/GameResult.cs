public abstract record GameResult(bool IsClear);

public record ClearResult(

    Score Score
    // Ç±Ç±Ç…ÉfÅ[É^

    ) : GameResult(IsClear: true);

public record FailedResult(
    
    //

    ) : GameResult(IsClear: false);
