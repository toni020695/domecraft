namespace DomeCraft.Data
{
    /// <summary>
    /// Enumerates the playable races available in DomeCraft.
    /// Add new races here and provide corresponding <see cref="RaceData"/>
    /// definitions via the static factory methods below.
    /// </summary>
    public enum RaceName
    {
        /// <summary>A versatile race with balanced stats and no strong weaknesses.</summary>
        Human,

        /// <summary>An ancient, long-lived race attuned to magic with naturally high mana.</summary>
        Elf,

        /// <summary>A stout, resilient race with high health and physical defence.</summary>
        Dwarf,
    }

    /// <summary>
    /// Immutable data container that holds the base stat modifiers and
    /// flavour information for a single race.
    ///
    /// Modifiers are additive bonuses applied on top of a character's
    /// <see cref="CharacterClassData"/> base stats during character creation.
    ///
    /// Extend this class or add new factory methods when introducing
    /// additional races (e.g. Orc, Gnome, Undead).
    /// </summary>
    public class RaceData
    {
        // ─── Properties ──────────────────────────────────────────────────────────

        /// <summary>The enum identifier for this race.</summary>
        public RaceName Race { get; }

        /// <summary>Human-readable display name shown in the UI.</summary>
        public string DisplayName { get; }

        /// <summary>Short lore description shown on the race-selection screen.</summary>
        public string Description { get; }

        /// <summary>Flat bonus added to a character's base health.</summary>
        public float HealthBonus { get; }

        /// <summary>Flat bonus added to a character's base mana.</summary>
        public float ManaBonus { get; }

        /// <summary>Flat bonus added to a character's base physical attack.</summary>
        public float AttackBonus { get; }

        /// <summary>Flat bonus added to a character's base magic power.</summary>
        public float MagicPowerBonus { get; }

        // ─── Constructor ─────────────────────────────────────────────────────────

        /// <summary>
        /// Initialises a <see cref="RaceData"/> with all required fields.
        /// </summary>
        private RaceData(
            RaceName race,
            string displayName,
            string description,
            float healthBonus,
            float manaBonus,
            float attackBonus,
            float magicPowerBonus)
        {
            Race           = race;
            DisplayName    = displayName;
            Description    = description;
            HealthBonus    = healthBonus;
            ManaBonus      = manaBonus;
            AttackBonus    = attackBonus;
            MagicPowerBonus = magicPowerBonus;
        }

        // ─── Factory Methods ─────────────────────────────────────────────────────

        /// <summary>
        /// Creates the default <see cref="RaceName.Human"/> data stub.
        /// Humans receive a small bonus to all stats (jack-of-all-trades).
        /// </summary>
        /// <returns>A new <see cref="RaceData"/> for the Human race.</returns>
        public static RaceData CreateHuman()
        {
            return new RaceData(
                race:            RaceName.Human,
                displayName:     "Human",
                description:     "Adaptable and ambitious, Humans excel in every discipline.",
                healthBonus:     10f,
                manaBonus:       10f,
                attackBonus:     2f,
                magicPowerBonus: 2f
            );
        }

        /// <summary>
        /// Creates the default <see cref="RaceName.Elf"/> data stub.
        /// Elves trade health for a significant mana and magic power bonus.
        /// </summary>
        /// <returns>A new <see cref="RaceData"/> for the Elf race.</returns>
        public static RaceData CreateElf()
        {
            return new RaceData(
                race:            RaceName.Elf,
                displayName:     "Elf",
                description:     "Graceful and magically gifted, Elves are natural spell-weavers.",
                healthBonus:     -10f,
                manaBonus:       30f,
                attackBonus:     1f,
                magicPowerBonus: 8f
            );
        }

        /// <summary>
        /// Creates the default <see cref="RaceName.Dwarf"/> data stub.
        /// Dwarves have high health and attack bonuses but very low mana.
        /// </summary>
        /// <returns>A new <see cref="RaceData"/> for the Dwarf race.</returns>
        public static RaceData CreateDwarf()
        {
            return new RaceData(
                race:            RaceName.Dwarf,
                displayName:     "Dwarf",
                description:     "Stubborn and powerful, Dwarves are unmatched in physical endurance.",
                healthBonus:     30f,
                manaBonus:       -10f,
                attackBonus:     5f,
                magicPowerBonus: -2f
            );
        }

        // ─── Utility ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override string ToString() =>
            $"[{Race}] HP+{HealthBonus} MP+{ManaBonus} ATK+{AttackBonus} MGK+{MagicPowerBonus}";
    }
}
