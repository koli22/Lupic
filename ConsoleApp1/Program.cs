Loop loop = new Loop();





class User
{
    public (int, int, int) getInputs()
    {
        Console.WriteLine("Jaka ma byt maximalni hodnota sperku?");
        int maxValue = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Kolik sperku ma byt v klenotnictvi?");
        int numOfChars = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Jak ma byt velky batoh");
        int maxBagValue = Convert.ToInt32(Console.ReadLine());

        return (maxValue, numOfChars, maxBagValue);
    }
}

class Loop
{
    private User user;
    private SperkySeznam seznam;

    public Loop()
    {
        user = new User();
        seznam = new SperkySeznam();

        this.loop();
    }

    public void loop()
    {
        (int, int, int) inputs = user.getInputs();
        seznam.generateNewBag(inputs.Item2, inputs.Item1);
        seznam.solve(new GreedySolver(), inputs.Item3);
        seznam.solve(new Alg1(), inputs.Item3);
        seznam.solve(new KnapsackSolver(), inputs.Item3);


        Console.WriteLine("Done");
        Console.ReadLine();
    }
}



class Alg1 : Algs
{
    public override Sperk[] SolveAlg(Sperk[] vyber, int maxSize)
    {
        List<Sperk> sperk = new List<Sperk> (vyber);
        List<Sperk> sperk_ = new List<Sperk> ();
        bool added = true;
        int misto = maxSize;

        while (added)
        {
            added = false;
            for (int i = 0; i < sperk.Count; i++)
                if (sperk[i].size <= misto && sperk[i].value > 0)
                {
                    misto-= sperk[i].size;
                    sperk_.Add(sperk[i]);
                    added = true;
                    sperk.Remove(sperk[i]);
                    i--;
                }
        }

        return sperk_.ToArray();
    }
}

class GreedySolver : Algs
{
    // SolveAlg method: Implements the greedy algorithm
    public override Sperk[] SolveAlg(Sperk[] vyber, int maxSize)
    {
        // Sort Sperk items by value-to-space ratio in descending order
        var sortedItems = vyber.OrderByDescending(item => (double)item.value / item.size).ToArray();

        List<Sperk> selectedItems = new List<Sperk>();
        int currentSpace = 0;

        // Iterate over the sorted items and select the ones that fit
        foreach (var item in sortedItems)
        {
            if (currentSpace + item.size <= maxSize)
            {
                // If the item fits, add it to the list
                selectedItems.Add(item);
                currentSpace += item.size;
            }
        }

        // Return the array of selected Sperk items
        return selectedItems.ToArray();
    }
}

class KnapsackSolver : Algs
{
    // SolveAlg method: Implements the dynamic programming algorithm
    public override Sperk[] SolveAlg(Sperk[] vyber, int maxSize)
    {
        int n = vyber.Length;
        int[,] dp = new int[n + 1, maxSize + 1];

        // Build the DP table
        for (int i = 1; i <= n; i++)
        {
            for (int w = 0; w <= maxSize; w++)
            {
                if (vyber[i - 1].size <= w)
                {
                    // Max value by either taking the item or not
                    dp[i, w] = Math.Max(dp[i - 1, w], dp[i - 1, w - vyber[i - 1].size] + vyber[i - 1].value);
                }
                else
                {
                    dp[i, w] = dp[i - 1, w];
                }
            }
        }

        // Backtrack to find the items that were included
        List<Sperk> result = new List<Sperk>();
        int remainingSpace = maxSize;
        for (int i = n; i > 0 && remainingSpace > 0; i--)
        {
            if (dp[i, remainingSpace] != dp[i - 1, remainingSpace])
            {
                // This item was included
                result.Add(vyber[i - 1]);
                remainingSpace -= vyber[i - 1].size;
            }
        }

        // Return the array of selected Sperk items
        return result.ToArray();
    }
}



abstract class Algs
{
    public void Solve(Sperk[] vyber, int maxSize)
    {
        Sperk[] sperky = this.SolveAlg(vyber, maxSize);

        int suma = 0;

        foreach (var i in sperky)
            suma += i.size;

        if (suma > maxSize)
        {
            Console.WriteLine("moc sperku v batohu");
            throw new OutOfMemoryException();
        }


        foreach (var i in sperky)
        {
            Console.WriteLine(i.ToString());
        }
    }
    abstract public Sperk[] SolveAlg(Sperk[] vyber, int maxSize);
}

class SperkySeznam
{
    private string[] sperky;
    private Sperk[] obsah;
    private Random rand;
    public SperkySeznam()
    {
        rand = new Random();
        sperky = new string[] {
            "Prsten",
    "Náhrdelník",
    "Náramek",
    "Náušnice",
    "Brož",
    "Přívěsek",
    "Řetízek",
    "Korále",
    "Manžetové knoflíky",
    "Diadém",
    "Koruna",
    "Čelenka",
    "Snubní prsten",
    "Zásnubní prsten",
    "Pečetní prsten"};
    }

    public void generateNewBag(int maxLen, int maxValue)
    {
        obsah = new Sperk[maxLen];

        for (int i = 0; i < obsah.Length; i++)
            obsah[i] = new Sperk(maxValue, rand, sperky);
    }

    public void solve(Algs alg, int maxSize)
    {
        Console.WriteLine(alg.GetType().Name);
        alg.Solve(obsah, maxSize);
    }
}


struct Sperk
{
    public int value { get; }
    public string name { get; }

    public int size { get; }

    public Sperk(int maxValue, Random rand, string[] sperky)
    {
        this.value = rand.Next(maxValue);
        this.name = sperky[rand.Next(sperky.Length)];
        this.size = rand.Next(maxValue) * 2;
    }


    public override string ToString()
    {
        return $"({name}, {value}, {size})";
    }
}