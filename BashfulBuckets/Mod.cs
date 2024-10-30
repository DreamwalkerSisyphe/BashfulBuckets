using GDWeave;
using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace BashfulBuckets;



public class Mod : IMod {
    
    public Mod(IModInterface modInterface) {
        modInterface.RegisterScriptMod(new ScriptMod());
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}

public class ScriptMod : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Props/fish_trap.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var readyWaiter = new FunctionWaiter("_ready");

        var hasFishWaiter = new MultiTokenWaiter([
            t => t is ConstantToken { Value: StringVariant { Value: "one of your buddies caught something!" } },
            t => t.Type == TokenType.ParenthesisClose,
            t => t.Type == TokenType.Newline]);

        var removeFishWaiter = new MultiTokenWaiter([
            t => t.Type == TokenType.CfElse,
            t => t.Type == TokenType.Colon,
            t => t.Type == TokenType.Newline]);

        foreach (var token in tokens)
        {
            if (readyWaiter.Check(token))
            {
                //Return new line
                yield return token;

                //Remove bucket from interactable group
                yield return new IdentifierToken("remove_from_group");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("interactable"));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else if (hasFishWaiter.Check(token))
            {
                //Return new line
                yield return new Token(TokenType.Newline, 1);

                //Add bucket back to interactable group
                yield return new IdentifierToken("add_to_group");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("interactable"));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else if (removeFishWaiter.Check(token))
            {
                //Return new line
                yield return token;

                //Remove bucket from interactable group
                yield return new IdentifierToken("remove_from_group");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("interactable"));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 2);
            }
            else
                yield return token;
        }
    }
}