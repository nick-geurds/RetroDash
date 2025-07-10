public interface ISpawnHittable
{
    void SetHittable(bool value);
    bool IsHittable();
    void SetWillBeHittable(bool value);  // nieuwe methode om willBeHittable te zetten
}