namespace Necromancy.Server.Model
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public byte viewOffset { get; set; }
        public int battlePose { get; set; }
        public byte charaPose { get; set; }
        public byte movementAnim { get; set; }
        public byte animJumpFall { get; set; }
        public byte xAnim { get; set; }
        public bool weaponEquipped { get; set; }
        public byte wepEastWestAnim { get; set; }
        public byte wepNorthSouthAnim { get; set; }

    }

}