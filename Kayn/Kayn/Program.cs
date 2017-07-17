using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Kayn
{
    internal class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted R;
        public static Menu Dicks, ComboMenu, DrawMenu, KsMenu, FarmMenu;
        private static AIHeroClient Kayn => EloBuddy.Player.Instance;

        public static AIHeroClient Player => ObjectManager.Player;

        private static void Main(string[] args)
        {
            Chat.Print("<font color='#add8e6'>Kayn Basic</font> Loaded.");
            Chat.Print("By <font color='#ff0000'>BestSkarnerNA</font>");
            Loading.OnLoadingComplete += LoadingKayn;
        }

        private static void LoadingKayn(EventArgs args)
        {
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

            Q = new Spell.Skillshot(SpellSlot.Q, 350, SkillShotType.Circular);
            W = new Spell.Skillshot(SpellSlot.W, 750, SkillShotType.Linear);
            R = new Spell.Targeted(SpellSlot.R, 425);

            Dicks = MainMenu.AddMenu("Kayn", "BestSkarnerNA");

            ComboMenu = Dicks.AddSubMenu("Combo", "Combo");

            ComboMenu.Add("Q", new CheckBox("Q"));
            ComboMenu.Add("W", new CheckBox("W"));
            ComboMenu.Add("R", new CheckBox("R", false));

            FarmMenu = Dicks.AddSubMenu("Farm", "SPells");
            FarmMenu.Add("W", new CheckBox("W"));


            KsMenu = Dicks.AddSubMenu("KS", "R");

            KsMenu.Add("R", new CheckBox("R"));

            DrawMenu = Dicks.AddSubMenu("Draw", "Colorful Drawings");

            DrawMenu.Add("Q", new CheckBox("Q"));
            DrawMenu.Add("W", new CheckBox("W"));
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo();
            if (KsMenu["R"].Cast<CheckBox>().CurrentValue)
                Ks();
        }


        private static void Ks()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (!KsMenu["R"].Cast<CheckBox>().CurrentValue) return;
            if (target == null || !(target.Health < Rdps(target))) return;
            if (!target.IsInRange(Player, R.Range) && R.IsReady())
                return;
            {
                R.Cast(target);
            }
        }

        public static float Rdps(Obj_AI_Base target) // made by Nao o shurima
        {
            if (R.IsReady())
                return EloBuddy.Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                    new[] {150f, 250f, 350f}[R.Level] + 1f * ObjectManager.Player.FlatPhysicalDamageMod +
                    0.15f * target.Health);
            return 0f;
        }


        private static void LaneClear()
        {
            var target = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(a => !a.IsDead && W.IsInRange(a));

            if (target == null)
                return;

            if (FarmMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                Q.Cast();
            if (FarmMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range))
                W.Cast();
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (DrawMenu["Q"].Cast<CheckBox>().CurrentValue)
                Circle.Draw(Color.Purple, Q.Range, Kayn);
            if (DrawMenu["W"].Cast<CheckBox>().CurrentValue)
                Circle.Draw(Color.Purple, W.Range, Kayn);
        }

        internal static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (!ComboMenu["W"].Cast<CheckBox>().CurrentValue) return;
            if (target.Distance(ObjectManager.Player) <= W.Range && W.IsReady())
                W.Cast(W.GetPrediction(target).CastPosition);

            target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!ComboMenu["Q"].Cast<CheckBox>().CurrentValue) return;
            if (target.Distance(ObjectManager.Player) <= Q.Range && Q.IsReady())
                Q.Cast(Q.GetPrediction(target).CastPosition);

            target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (!ComboMenu["R"].Cast<CheckBox>().CurrentValue) return;
            if (target.Distance(ObjectManager.Player) <= R.Range && R.IsReady())
                R.Cast(target);
        }
    }
}
