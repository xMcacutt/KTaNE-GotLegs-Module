using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using KModkit;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Quirk
{
    SingleAmputee,
    ExtraLimb,
    HalfLegsFellOff,
    WearingFunnyHat,
    Old,
    Pregnant,
    PregnantWithTwins,
    Sad,
    Boots,
    Flight
}

public struct Entry
{
    public int Count;
    public string Animal;
    public Quirk Quirk;
    public bool UseUnique;
}

public class Legs : Module
{
    public KMSelectable leftButton;
    public KMSelectable rightButton;
    public KMSelectable submitButton;
    public KMSelectable clearButton;

    public TextMesh countText;
    public TextMesh animalText;
    public TextMesh quirkText;
    public TextMesh extraQuirkText;

    public KMSelectable[] numberButtons;

    private readonly Dictionary<string, int> _legsCount = new Dictionary<string, int>()
    {
        { "Mountain Chicken", 4 },
        { "Hellbender", 4 },
        { "Secretary Bird", 2 },
        { "Peacock Mantis Shrimp", 16 },
        { "Bearcat", 4 },
        { "Rock Hyrax", 4 },
        { "Turtle Dove", 2 },
        { "Blunt-Tailed Snake", 100 },
        { "Flamingo", 2 },
        { "Kangaroo", 3 },
        { "Symphyla", 24 },
        { "Blue Foot Baboon", 8 },
        { "Dog", 4 },
        { "Human", 2 },
        { "Python", 0 }
    };

    private readonly Dictionary<string, int> _handCount = new Dictionary<string, int>()
    {
        { "Mountain Chicken", 2 },
        { "Hellbender", 0 },
        { "Secretary Bird", 0 },
        { "Peacock Mantis Shrimp", 0 },
        { "Bearcat", 0 },
        { "Rock Hyrax", 0 },
        { "Turtle Dove", 0 },
        { "Blunt-Tailed Snake", 0 },
        { "Flamingo", 0 },
        { "Kangaroo", 2 },
        { "Symphyla", 0 },
        { "Blue Foot Baboon", 0 },
        { "Dog", 0 },
        { "Human", 2 },
        { "Python", 0 }
    };

    private readonly Dictionary<string, int> _wingCount = new Dictionary<string, int>()
    {
        { "Mountain Chicken", 0 },
        { "Hellbender", 0 },
        { "Secretary Bird", 2 },
        { "Peacock Mantis Shrimp", 0 },
        { "Bearcat", 0 },
        { "Rock Hyrax", 0 },
        { "Turtle Dove", 2 },
        { "Blunt-Tailed Snake", 0 },
        { "Flamingo", 2 },
        { "Kangaroo", 0 },
        { "Symphyla", 0 },
        { "Blue Foot Baboon", 0 },
        { "Dog", 0 },
        { "Human", 0 },
        { "Python", 0 }
    };

    private readonly Dictionary<string, string> _uniqueQuirksSingle = new Dictionary<string, string>()
    {
        { "Mountain Chicken", "but it's a chicken\non a mountain" },
        { "Hellbender", "but it thinks it's a dragon" },
        { "Secretary Bird", "but that's its job" },
        { "Peacock Mantis Shrimp", "but that's a\nlist of three animals" },
        { "Bearcat", "but the bear\nate the cat" },
        { "Rock Hyrax", "awawa" },
        { "Turtle Dove", "but the dove flew off" },
        { "Blunt-Tailed Snake", "wait no, that's just a worm" },
        { "Flamingo", "but it's standing on one leg" },
        { "Kangaroo", "but there's a joey in its pouch" },
        { "Symphyla", "but it wants a hug" },
        { "Blue Foot Baboon", "but it has cold feet" },
        { "Dog", "but it brought\nyou a present" },
        { "Human", "but it's in a\nthree-legged race" },
        { "Python", "but it's the\nprogramming language" }
    };

    private readonly Dictionary<Quirk, string> _quirksSingle = new Dictionary<Quirk, string>()
    {
        { Quirk.SingleAmputee, "but its a\nsingle amputee" },
        { Quirk.ExtraLimb, "with an extra Limb" },
        { Quirk.HalfLegsFellOff, "but half its\nlegs fell off" },
        { Quirk.WearingFunnyHat, "but its wearing\na silly hat" },
        { Quirk.Old, "but it's really,\nreally old" },
        { Quirk.Pregnant, "but it's pregnant! YAY!" },
        { Quirk.PregnantWithTwins, "but it's pregnant\nwith twins! Oh no..." },
        { Quirk.Sad, "but it's sad :(" },
        { Quirk.Boots, "but it's wearing the\nbattery holders as boots" },
        { Quirk.Flight, "but it's migrating back home" }
    };
    
    private readonly Dictionary<string, string> _uniqueQuirksPlural = new Dictionary<string, string>()
    {
        { "Mountain Chicken", "but they're a chicken\non a mountain" },
        { "Hellbender", "but they think they're a dragon" },
        { "Secretary Bird", "but that's a job title" },
        { "Peacock Mantis Shrimp", "but that's a\nlist of three animals" },
        { "Bearcat", "but the bear\nate the cat" },
        { "Rock Hyrax", "awawa" },
        { "Turtle Dove", "but the dove flew off" },
        { "Blunt-Tailed Snake", "wait no, that's just some worms" },
        { "Flamingo", "but they're standing on one leg" },
        { "Kangaroo", "but there's a joey in their pouch" },
        { "Symphyla", "but they want a hug" },
        { "Blue Foot Baboon", "but they have cold feet" },
        { "Dog", "but they brought\nyou presents" },
        { "Human", "but they're in a\nthree-legged race" },
        { "Python", "but they're the\nprogramming language" }
    };

    private readonly Dictionary<Quirk, string> _quirksPlural = new Dictionary<Quirk, string>()
    {
        { Quirk.SingleAmputee, "but they're\nsingle amputees" },
        { Quirk.ExtraLimb, "with an extra Limb" },
        { Quirk.HalfLegsFellOff, "but half their\nlegs fell off" },
        { Quirk.WearingFunnyHat, "but they're wearing\nsilly hats" },
        { Quirk.Old, "but they're really,\nreally old" },
        { Quirk.Pregnant, "but they're pregnant! YAY!" },
        { Quirk.PregnantWithTwins, "but they're pregnant\nwith twins! Oh no..." },
        { Quirk.Sad, "but they're sad :(" },
        { Quirk.Boots, "but they're wearing the\nbattery holders as boots" },
        { Quirk.Flight, "but they're migrating back home" }
    };
    
    private List<Entry> Entries = new List<Entry>();
    private int currentIndex = 0;
    private int currentInputValue = 0;
    private int LegCount = 0;
    private readonly System.Random _random = new System.Random();
    
    protected override void ModuleStart()
    {
        name = "Got Legs?";
        var shuffledAnimals = _legsCount.Keys.OrderBy(x => _random.Next()).ToList();
        for (var i = 0; i < 5; i++)
        {
            Entries.Add(new Entry
            {
                Count = _random.Next(1, 6), Animal = shuffledAnimals[i], Quirk = (Quirk)_random.Next(8),
                UseUnique = _random.Next(5) == 0
            });
        }

        leftButton.OnInteract += HandlePageLeft;
        rightButton.OnInteract += HandlePageRight;

        for (var i = 0; i < numberButtons.Length; i++)
        {
            var index = i;
            numberButtons[i].OnInteract += () => HandleNumberPressed(index);
        }

        submitButton.OnInteract += HandleSubmit;
        clearButton.OnInteract += HandleClear;
        
        UpdateScreens();

        LegCount = Entries.Aggregate(0, (current, entry) => current + GetActualCount(entry.Animal, entry.Quirk, entry.Count, entry.UseUnique));
        LegCount = Math.Max(LegCount, 0);
        Debug.LogFormat("[Got Legs? #{0}] Legs count: {1}", moduleId, LegCount);
    }

    private bool HandleSubmit()
    {
        submitButton.AddInteractionPunch();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitButton.transform);
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, leftButton.transform);
        if (_isSolved)
            return false;
        if (currentInputValue == LegCount)
        {
            module.HandlePass();
            audio.PlaySoundAtTransform("Solve", module.transform);
            quirkText.text = "Yeah, I got legs.";
            extraQuirkText.text = "";
            animalText.text = "Got legs?";
            countText.text = "gSsJBO2JUO4";
        }
        else
        {
            module.HandleStrike();
            audio.PlaySoundAtTransform("Strike", module.transform);
            Debug.LogFormat("[Got Legs? #{0}] Submitted: {1}", moduleId, currentInputValue);
            currentInputValue = 0;
        }
        return false;
    }

    private bool HandleClear()
    {
        clearButton.AddInteractionPunch();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, clearButton.transform);
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, leftButton.transform);
        if (_isSolved)
            return false;
        currentIndex = 0;
        return false;
    }

    private bool HandlePageLeft()
    {
        leftButton.AddInteractionPunch();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, leftButton.transform);
        if (_isSolved)
            return false;
        currentIndex = (currentIndex - 1).Mod(5);
        UpdateScreens();
        return false;
    }

    private bool HandlePageRight()
    {
        rightButton.AddInteractionPunch();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, rightButton.transform);
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, leftButton.transform);
        if (_isSolved)
            return false;
        currentIndex = (currentIndex + 1).Mod(5);
        UpdateScreens();
        return false;
    }

    private bool HandleNumberPressed(int number)
    {
        numberButtons[number].AddInteractionPunch();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, numberButtons[number].transform);
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, leftButton.transform);
        if (_isSolved)
            return false;
        currentInputValue *= 10;
        currentInputValue += number;
        return false;
    }
    
    private int GetActualCount(string animal, Quirk quirk, int animalCount, bool useUnique)
    {
        var legCount = _legsCount[animal];
        var handCount = _handCount[animal];
        var wingCount = _wingCount[animal];
        if (useUnique)
        {
            switch (animal)
            {
                case "Mountain Chicken":
                    legCount = 2;
                    handCount = 0;
                    wingCount = 2;
                    break;
                case "Hellbender":
                    legCount = 4;
                    handCount = 0;
                    wingCount = 2;
                    break;
                case "Secretary Bird":
                    legCount = 1;
                    handCount = 0;
                    wingCount = 2;
                    break;
                case "Peacock Mantis Shrimp":
                    legCount = 28;
                    wingCount = 2;
                    handCount = 0;
                    break;
                case "Bearcat":
                    legCount = 8;
                    handCount = 0;
                    wingCount = 0;
                    break;
                case "Rock Hyrax":
                    break;
                case "Turtle Dove":
                    legCount = 4;
                    handCount = 0;
                    wingCount = 0;
                    break;
                case "Blunt-Tailed Snake":
                    legCount = 0;
                    handCount = 0;
                    wingCount = 0;
                    break;
                case "Flamingo":
                    legCount = 1;
                    handCount = 1;
                    wingCount = 2;
                    break;
                case "Kangaroo":
                    legCount = 6;
                    handCount = 4;
                    wingCount = 0;
                    break;
                case "Symphyla":
                    legCount -= 2;
                    handCount += 2;
                    wingCount = 0;
                    break;
                case "Blue Foot Baboon":
                    legCount -= 4;
                    handCount = 0;
                    wingCount = 0;
                    break;
                case "Dog":
                    legCount += 2;
                    handCount = 0;
                    wingCount += 2;
                    break;
                case "Human":
                    legCount += 1;
                    break;
                case "Python":
                    legCount = -10;
                    break;
            }
        }
        switch (quirk)
        {
            case Quirk.ExtraLimb:
                legCount += 1;
                break;
            case Quirk.HalfLegsFellOff:
                legCount /= 2;
                break;
            case Quirk.WearingFunnyHat:
                legCount += int.Parse(bomb.GetSerialNumber().First(char.IsDigit).ToString());
                break;
            case Quirk.Old: 
                legCount += handCount;
                if (handCount == 0)
                    legCount = 0;
                break;
            case Quirk.Pregnant:
                legCount *= 2;
                break;
            case Quirk.PregnantWithTwins:
                legCount *= 3;
                break;
            case Quirk.Sad:
                legCount = 0;
                break;
            case Quirk.SingleAmputee:
                legCount -= 1;
                break;
            case Quirk.Boots:
                legCount += bomb.GetBatteryCount();
                break;
            case Quirk.Flight:
                legCount += wingCount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(quirk), quirk, null);
        }

        if (useUnique)
            animal += " with unique quirk";
        Debug.LogFormat("[Got Legs? #{0}] {1} {2} {3} have {4} legs", moduleId, animalCount, animal, quirk, legCount * animalCount);
        return legCount * animalCount;
    }

    private void UpdateScreens()
    {
        var entry = Entries[currentIndex];
        countText.text = entry.Count.ToString();
        animalText.text = entry.Animal;
        animalText.gameObject.transform.localPosition = new Vector3(0, 0.501f, entry.UseUnique ? 0.1f : 0);
        extraQuirkText.text = entry.UseUnique ? (entry.Count > 1 ? _uniqueQuirksPlural[entry.Animal]: _uniqueQuirksSingle[entry.Animal]) : "";
        var quirkTextUnformatted = entry.Count > 1 ? _quirksPlural[entry.Quirk]: _quirksSingle[entry.Quirk];
        if (entry.UseUnique)
        {
            if (quirkTextUnformatted.StartsWith("but ", StringComparison.Ordinal))
                quirkTextUnformatted = quirkTextUnformatted.Substring(4);
            quirkTextUnformatted = "and " + quirkTextUnformatted;
        }
        quirkText.text = quirkTextUnformatted;
    }
    
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} l/r (page left/right), !{0} sub (submit), !{0} clr (clear), !{0} <numbers> (number buttons)";
#pragma warning restore 414
	
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        if (Regex.IsMatch(command, @"^\s*(?:sub)\s*$", RegexOptions.IgnoreCase))
        {
            yield return null;
            HandleSubmit();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*(?:clr)\s*$", RegexOptions.IgnoreCase))
        {
            yield return null;
            HandleClear();
            yield break;
        }
        var match = Regex.Match(command, @"^\s*(\d+)\s*$", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            yield return null;
            foreach (var digit in match.Groups[1].Value.Select(x => int.Parse(x.ToString())))
                HandleNumberPressed(digit);
            yield break;
        }
        match = Regex.Match(command, @"^\s*(?:input) (\D+)\s*$", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            yield return null;
            var direction = match.Groups[1].Value.ToLower();
            if (direction == "l")
                HandlePageLeft();
            else if (direction == "r")
                HandlePageRight();
            yield break;
        }
        yield return null;
    }
	
    IEnumerator TwitchHandleForcedSolve()
    {
        Debug.LogFormat("[The Fan #{0}] Forced solve", moduleId);
        audio.PlaySoundAtTransform("AutoSolve", module.transform);
        _isSolved = true;
        quirkText.text = "Yeah, I got legs.";
        extraQuirkText.text = "gSsJBO2JUO4";
        animalText.text = "Got legs?";
        countText.text = "";
        module.HandlePass();
        yield break;
    }
}
