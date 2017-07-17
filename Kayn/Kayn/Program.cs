using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using SharpDX;
using EloBuddy.SDK.Rendering;

namespace Kayn
{
    class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted R;
        public static Menu Dicks, ComboMenu, DrawMenu, KSMenu, FarmMenu;
        private static AIHeroClient Kayn => Player.Instance;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += LoadingKayn;
        }

        private static void LoadingKayn(EventArgs args)
        {

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

            Q = new Spell.Skillshot(SpellSlot.Q, 350, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            W = new Spell.Skillshot(SpellSlot.W, 750, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            R = new Spell.Targeted(SpellSlot.R, 425);

            Dicks = MainMenu.AddMenu("Dicks", "Fat Cocks");

            ComboMenu = Dicks.AddSubMenu("Combo", "Combo");

            ComboMenu.Add("Q", new CheckBox("Q"));
            ComboMenu.Add("W", new CheckBox("W"));
            ComboMenu.Add("R", new CheckBox("R"));

            FarmMenu = Dicks.AddSubMenu("Farm", "Spells");
            FarmMenu.Add("W", new CheckBox("W"));
            FarmMenu.Add("R", new CheckBox("R"));


            DrawMenu = Dicks.AddSubMenu("Draw", "Fat Dick Draw");

            DrawMenu.Add("Q", new CheckBox("Q"));
            DrawMenu.Add("W", new CheckBox("W"));

        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                LaneClear();
            }
        }

        private static void LaneClear()
        {
            var minions =
                EntityManager.MinionsAndMonsters
                    .GetLaneMinions(EntityManager.UnitTeam.Enemy, EloBuddy.Player.Instance.Position, W.Range)
                    .Where(m => !m.IsDead && m.IsValid && !m.IsInvulnerable);

            {
                foreach (var m in minions)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    {
                        if (FarmMenu["Q"].Cast<CheckBox>().CurrentValue)
                        {
                            Q.Cast();
                        }
                        if (FarmMenu["W"].Cast<CheckBox>().CurrentValue)
                        {
                            W.Cast();
                        }
                    }
                }
            }

        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (DrawMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Aqua, Q.Range, Kayn);
            }
            if (DrawMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Aqua, W.Range, Kayn);

            }
        }

        internal static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (ComboMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                if (target.Distance(ObjectManager.Player) <= W.Range && W.IsReady())
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }

                target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (ComboMenu["Q"].Cast<CheckBox>().CurrentValue)
                {
                    if (target.Distance(ObjectManager.Player) <= Q.Range && Q.IsReady())
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }

                    target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    if (ComboMenu["R"].Cast<CheckBox>().CurrentValue)
                    {
                        if (target.Distance(ObjectManager.Player) <= R.Range && R.IsReady())
                        {
                            R.Cast(target);
                        }

                    }
                }
            }
        }
    }
}
