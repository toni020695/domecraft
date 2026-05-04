namespace DomeCraft.Data
{
    /// <summary>
    /// Enumerates the playable character classes available in DomeCraft.
    /// Add new classes here and provide corresponding <see cref="CharacterClassData"/>
    /// definitions below.
    /// </summary>
    public enum CharacterClassName
    {
        /// <summary>A heavily armoured melee fighter with high health and defence.</summary>
        Warrior,

        /// <summary>A spell-casting class with high mana and elemental damage output.</summary>
        Mage,
    }

    /// <summary>
    /// Immutable data container that defines the base stats and flavour text
    /// for a single character class.
    ///
    /// Instances are created via the static factory methods
    /// <see cref="CreateWarrior"/> and <see cref="CreateMage"/>.
    /// Extend this class or add new factory methods when introducing
    /// additional classes (e.g. Rogue, Paladin).
    /// </summary>
    public class CharacterClassData
    {
        // ─── Properties ──────────────────────────────────────────────────────────

        /// <summary>The enum identifier for this class.</summary>
        public CharacterClassName ClassName { get; }

        /// <summary>Human-readable display name shown in the UI.</summary>
        public string DisplayName { get; }

        /// <summary>Short description shown on the class-selection screen.</summary>
        public string Description { get; }

        /// <summary>Base health points before race/equipment modifiers.</summary>
        public float BaseHealth { get; }

        /// <summary>Base mana points before race/equipment modifiers.</summary>
        public float BaseMana { get; }

        /// <summary>Base physical attack power.</summary>
        public float BaseAttack { get; }

        /// <summary>Base magic power used for spell damage calculations.</summary>
        public float BaseMagicPower { get; }

        // ─── Constructor ─────────────────────────────────────────────────────────

        /// <summary>
        /// Initialises a <see cref="CharacterClassData"/> with all required fields.
        /// </summary>
        private CharacterClassData(
            CharacterClassName className,
            string displayName,
            string description,
            float baseHealth,
            float baseMana,
            float baseAttack,
            float baseMagicPower)
        {
            ClassName     = className;
            DisplayName   = displayName;
            Description   = description;
            BaseHealth    = baseHealth;
            BaseMana      = baseMana;
            BaseAttack    = baseAttack;
            BaseMagicPower = baseMagicPower;
        }

        // ─── Factory Methods ─────────────────────────────────────────────────────

        /// <summary>
        /// Creates the default <see cref="CharacterClassName.Warrior"/> data stub.
        /// Tune the values here as game-balance decisions are made.
        /// </summary>
        /// <returns>A new <see cref="CharacterClassData"/> for the Warrior class.</returns>
        public static CharacterClassData CreateWarrior()
        {
            return new CharacterClassData(
                className:      CharacterClassName.Warrior,
                displayName:    "Warrior",
                description:    "A battle-hardened fighter who relies on strength and heavy armour.",
                baseHealth:     150f,
                baseMana:       20f,
                baseAttack:     15f,
                baseMagicPower: 2f
            );
        }

        /// <summary>
        /// Creates the default <see cref="CharacterClassName.Mage"/> data stub.
        /// Tune the values here as game-balance decisions are made.
        /// </summary>
        /// <returns>A new <see cref="CharacterClassData"/> for the Mage class.</returns>
        public static CharacterClassData CreateMage()
        {
            return new CharacterClassData(
                className:      CharacterClassName.Mage,
                displayName:    "Mage",
                description:    "A scholar of arcane arts who wields devastating spells at the cost of low health.",
                baseHealth:     70f,
                baseMana:       120f,
                baseAttack:     4f,
                baseMagicPower: 20f
            );
        }

        // ─── Utility ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override string ToString() =>
            $"[{ClassName}] HP:{BaseHealth} MP:{BaseMana} ATK:{BaseAttack} MGK:{BaseMagicPower}";
    }
}
