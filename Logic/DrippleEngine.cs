using System;
using System.Collections.Generic;
using System.Linq;

namespace Dripple.Logic
{
    public class Player
    {
        public Guid Address { get; set; }

        public double Payout { get; set; }

        public double AmmountOfTokens { get; set; }

        public int Rolls { get; set; }

        public Player(Guid address)
        {
            Address = address;
        }

        private Player()
        {

        }
    }

    public class DrippleEngine
    {
        public const double entryFee = 10;

        public const double exitFee = 10;

        public const double dripFee = 80;

        public const double payoutRate = 2;

        public double magnitude = Math.Pow(2, 64);

        public const double balanceInterval = 6; // hours

        public const double distributionInterval = 2; // seconds

        public DateTime LastPayout = DateTime.Now;

        public List<Player> Players = new List<Player>();

        public double TokenSupply = 0;

        public double ProfitPerShare = 0;

        public double TotalDeposits = 0;

        public double LastBalance = 0;

        public double TotalTransactions = 0;

        public double DividendBalance = 0;

        public double TotalClaims = 0;

        public DrippleEngine()
        {

        }

        public double Buy(Guid address, double buyAmmount)
        {
            TotalDeposits += buyAmmount;

            double ammount = PurchaseTokens(address, buyAmmount);

            Distribute();

            return ammount;
        }

        public void DonatePool(double ammount)
        {
            DividendBalance += ammount;
        }

        public void Reinvest(Guid address)
        {
            double dividends = DividendsOf(address);

            Players.First(x => x.Address == address).Payout += dividends * magnitude;

            Players.First(x => x.Address == address).Rolls++;

            double tokens = PurchaseTokens(address, dividends);

            Distribute();
        }

        public void Withdraw(Guid address)
        {
            double dividends = DividendsOf(address);

            Players.First(x => x.Address == address).Payout += dividends * magnitude;

            TotalTransactions++;

            TotalClaims += dividends;

            Distribute();
        }

        public void Sell(Player player, double ammountOfTokens)
        {
            double undividedDividends = (ammountOfTokens * exitFee) / 100;

            double taxedEtherium = ammountOfTokens - undividedDividends;

            // burn the sold tokens
            TokenSupply -= ammountOfTokens;

            Players.First(x => x.Address == player.Address).AmmountOfTokens -= ammountOfTokens;

            double updatedPayouts = ProfitPerShare * ammountOfTokens + (taxedEtherium * magnitude);

            Players.First(x => x.Address == player.Address).AmmountOfTokens -= updatedPayouts;

            AllocateFees(undividedDividends);

            Distribute();
        }

        public bool Transfers(Guid from, Guid to, double ammountOfTokens)
        {
            if (DividendsOf(from) > 0)
            {
                Withdraw(from);
            }

            Players.First(x => x.Address == from).AmmountOfTokens -= ammountOfTokens;
            Players.First(x => x.Address == to).AmmountOfTokens += ammountOfTokens;

            Players.First(x => x.Address == from).Payout -= ProfitPerShare * ammountOfTokens;
            Players.First(x => x.Address == to).Payout += ProfitPerShare * ammountOfTokens;

            TotalTransactions++;

            return true;
        }

        public double DividendsOf(Guid address)
        {
            Player player = Players.First(x => x.Address.Equals(address));

            double dividends = ((ProfitPerShare * player.AmmountOfTokens) - player.Payout) / magnitude;

            return dividends;
        }

        public Player GetPlayer(Guid address)
        {
            Player player = Players.First(x => x.Address == address);

            return player;
        }

        public double DailyEstimate(Player player)
        {
            double share = DividendBalance * payoutRate / 100;

            return TokenSupply > 0 ? share * player.AmmountOfTokens / TokenSupply : 0;
        }

        public double SellPrice()
        {
            double eth = 1e18;

            double dividends = eth * exitFee / 100;

            double taxedEth = eth - dividends;

            return taxedEth;
        }

        public double BuyPrice()
        {
            double eth = 1e18;

            double dividends = eth * entryFee / 100;

            double taxedEth = eth + dividends;

            return taxedEth;
        }

        public double CalculateTokensReceived(double ethToSpend)
        {
            double dividends = ethToSpend * entryFee / 100;

            double taxedEth = ethToSpend - dividends;

            double ammountOfTokens = taxedEth;

            return ammountOfTokens;
        }

        public double CalculateEthReceived(double tokensToSell)
        {
            double eth = tokensToSell;

            double dividends = eth * exitFee / 100;

            double taxedEth = eth - dividends;

            return taxedEth;
        }

        private double PurchaseTokens(Guid address, double buyAmmount)
        {
            if (!Players.Select(x => x.Address).ToList().Contains(address))
            {
                Players.Add(new Player(address));
            }

            Player player = Players.First(x => x.Address.Equals(address));

            TotalTransactions++;

            double undividedDividends = (buyAmmount * entryFee) / 100;

            double ammountOfTokens = buyAmmount - undividedDividends;

            if (TokenSupply > 0)
            {
                TokenSupply += ammountOfTokens;
            }
            else
            {
                TokenSupply = ammountOfTokens;
            }

            AllocateFees(undividedDividends);

            double updatedPayouts = ProfitPerShare * ammountOfTokens;

            

            Players.First(x => x.Address == player.Address).Payout += updatedPayouts;
            Players.First(x => x.Address == player.Address).AmmountOfTokens = ammountOfTokens;

            return ammountOfTokens;
        }

        private void AllocateFees(double fee)
        {
            // 1/5 paid out instantly

            double instant = fee / 5;

            if (TokenSupply > 0)
            {
                ProfitPerShare = (instant * magnitude) / TokenSupply;
            }

            DividendBalance += fee - instant;
        }

        private void Distribute()
        {
            // figure out the logic on when to distribute

            double share = ((DividendBalance * payoutRate) / 100) / 24;

            double profit = share * DateTime.Now.Subtract(LastPayout).Seconds;

            DividendBalance -= profit;

            ProfitPerShare += ((profit * magnitude) / TokenSupply);

            LastPayout = DateTime.Now;
        }
    }
}