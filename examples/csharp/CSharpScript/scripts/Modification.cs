RegisterModification(OTAPI.ModType.PostPatch, "Test", () => {
    //System.Console.WriteLine($"Modder: {Modder?.GetType()?.FullName ?? "No modder"}");
    System.Console.WriteLine($"Callback");
});