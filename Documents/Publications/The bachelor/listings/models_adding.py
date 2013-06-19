class Moves(models.Model):
    trainee = models.ForeignKey(Practitioner)
    name = models.CharField(max_length=128)
    count = models.IntegerField(null=True, blank=True,default=0)